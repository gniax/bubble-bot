using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BubbleBot.Configurations;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Accounts.InGame.Character.Inventory;
using BubbleBot.Core.Logs;
using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Enums;
using BubbleBot.Utility.DofusTouch;
using Microsoft.Win32;

namespace BubbleBot.Views.Accounts
{
    public partial class AccountConsoleUc
    {
        // Constructor
        public AccountConsoleUc()
        {
            InitializeComponent();
            ContextMenus = new List<ContextMenu>();

            ((INotifyCollectionChanged) Logs.Items).CollectionChanged += Logs_CollectionChanged;
        }

        // Properties
        private Account Account => BubbleBotMain.Instance.SelectedAccount;
        private List<ContextMenu> ContextMenus { get; }

        private async void Logs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (GlobalConfiguration.Instance.DisplayItemsInLogs)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var msg = e.NewItems[0] as LogMessage;
                    // If message contains objects...
                    if (msg.ObjectItems != null && msg.ObjectItems?.Count > 0 && msg.Message.Contains("\uFFFC"))
                        await Task.Run(async () =>
                        {
                            // Here we have to retrieve textblock to update it with content
                            var cp = Logs.ItemContainerGenerator.ContainerFromItem(e.NewItems[0]) as ContentPresenter;
                            TextBlock messageBlock = null;
                            await cp.Dispatcher.InvokeAsync(delegate
                            {
                                cp.ApplyTemplate();
                                messageBlock = (TextBlock) cp.ContentTemplate.FindName("LogsMessageTextBlock", cp);
                            });

                            if (messageBlock != null)
                            {
                                string content = null;
                                messageBlock.Dispatcher.Invoke(delegate
                                {
                                    content = messageBlock.Text;
                                    messageBlock.Text = ""; // clear the textbox to fill it later
                                });

                                var positionsObjects = new List<int>();
                                // Here we store every object position in order to insert them later (\uFFFC => Object Unicode char)
                                var regex = new Regex(Regex.Escape("\uFFFC"));
                                for (var i = 0; i < msg.ObjectItems?.Count; i++)
                                {
                                    positionsObjects.Add(regex.Match(content, 1).Index);
                                    content = regex.Replace(content, "", 1);
                                }

                                // Explanation :
                                // We add every text between objects to textblock thanks to previous objects positions
                                // To match with positions, we have to update the tmp list by introducing the object name into
                                // While this is not the last object, we update the next position by adding the item length
                                // Otherwise we add the remaining message 
                                // Exception: if the object is at the beginning (pos 0) we just have to add it directly
                                // index: message begin position to add <=> message: content between objects
                                // position: current object position to add in the sentence
                                var index = 0;
                                var tmp = content;
                                for (var i = 0; i < positionsObjects.Count; i++)
                                {
                                    var position = positionsObjects[i];
                                    var registeredItem =
                                        ObjectEnumFinder.GetObjectNameById((int) msg.ObjectItems[i].ObjectGID);
                                    var item = DataManager.Get<Items>((int) msg.ObjectItems[i].ObjectGID);
                                    var itemEntry = new ObjectEntry(msg.ObjectItems[i], item);
                                    var itemName = "[" + itemEntry.Name + "]";

                                    if (position > 0)
                                    {
                                        var messageToAdd = tmp.Substring(index, position - index);

                                        messageBlock.Dispatcher.Invoke(delegate
                                        {
                                            messageBlock.Inlines.Add(new Run(messageToAdd));
                                        });
                                        AddItemToLog(messageBlock, msg, itemEntry);

                                        tmp = tmp.Insert(position, itemName); // Keep update the temporary content
                                        index = position + itemName.Length;

                                        if (i != positionsObjects.Count - 1)
                                            for (var j = i + 1; j < positionsObjects.Count; j++)
                                                positionsObjects[j] += itemName.Length;
                                    }
                                    else
                                    {
                                        tmp = tmp.Insert(0, itemName);
                                        AddItemToLog(messageBlock, msg, itemEntry);
                                        index = itemName.Length;
                                    }
                                }

                                if (tmp.Length - index > 0)
                                {
                                    var textToAdd = tmp.Substring(index);
                                    messageBlock.Dispatcher.Invoke(delegate
                                    {
                                        messageBlock.Inlines.Add(new Run(textToAdd));
                                    });
                                }
                            }
                        });
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    // dispose textblock and childs / events
                }
            }
        }

        private void AddItemToLog(TextBlock messageBlock, LogMessage msg, ObjectEntry item)
        {
            messageBlock.Dispatcher.Invoke(delegate
            {
                var run = new Run("[" + item.Name + "]");
                var bold = new Bold(run);
                messageBlock.Inlines.Add(bold);
                run.MouseLeftButtonUp += (sender, e) => LogItem_Click(sender, e, messageBlock, msg, item);
                run.MouseEnter += ObjectLog_MouseEnter;
                run.MouseLeave += ObjectLog_MouseLeave;
            });
        }

        private void LogItem_Click(object sender, MouseEventArgs e, TextBlock parent, LogMessage msg, ObjectEntry item)
        {
            var contextMenu = new ContextMenu
            {
                BorderThickness = new Thickness(0, 0, 0, 0),
                HasDropShadow = false,
                Background = new SolidColorBrush(Colors.Black) { Opacity = 0.65 }
            };

            // First Menu Item => name/lvl/icon
            var menuItem1 = new MenuItem
            {
                Style = Application.Current.Resources["CustomMenuItem"] as Style
            };

            var grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            var rowDef = new RowDefinition {Height = new GridLength(1.0, GridUnitType.Star)};
            var colDef1 = new ColumnDefinition {Width = new GridLength(0.6, GridUnitType.Star)};
            var colDef2 = new ColumnDefinition {Width = new GridLength(0.2, GridUnitType.Star)};
            var colDef3 = new ColumnDefinition {Width = new GridLength(0.2, GridUnitType.Star)};
            grid.RowDefinitions.Add(rowDef);
            grid.ColumnDefinitions.Add(colDef1);
            grid.ColumnDefinitions.Add(colDef2);
            grid.ColumnDefinitions.Add(colDef3);

            // Here we set the name inside grid
            var intro = new TextBlock
            {
                Margin = new Thickness(-10, 15, 0, 0)
            };
            intro.Inlines.Add(new Run {FontWeight = FontWeights.DemiBold, Text = item.Name});
            intro.Inlines.Add(new LineBreak());
            intro.Inlines.Add(new Run {Text = LanguageManager.Translate("262") + " " + item.Level});

            var iconBg = new Rectangle {Width = 60, Height = 60, Opacity = 0.65};
            iconBg.Margin = new Thickness(35, 15, 0, 0);
            iconBg.Fill = new SolidColorBrush(Color.FromRgb(60, 58, 50));
            iconBg.Clip = new RectangleGeometry {Rect = new Rect(0, 0, 60, 60), RadiusX = 4, RadiusY = 4};
            Grid.SetColumn(iconBg, 1);

            var img = new Image
            {
                Margin = new Thickness(35, 15, 0, 0),
                Width = 50,
                Height = 50,
                Source = new BitmapImage(new Uri(item.IconUrl))
            };
            Grid.SetColumn(img, 1);

            var closeBtn = new Image
            {
                Margin = new Thickness(0, -43, -23, 0),
                Width = 32,
                Height = 32,
                Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/close_btn.png"))
            };
            Grid.SetColumn(closeBtn, 2);

            closeBtn.MouseEnter += CloseBtn_MouseEnter;
            closeBtn.MouseLeave += CloseBtn_MouseLeave;
            closeBtn.MouseLeftButtonUp += (snd, evnt) => CloseBtn_Click(snd, evnt, contextMenu);

            grid.Children.Add(intro);
            grid.Children.Add(iconBg);
            grid.Children.Add(img);
            grid.Children.Add(closeBtn);

            menuItem1.MouseEnter += MenuItem1_MouseEnter;
            menuItem1.Header = grid; // add grid with all
            menuItem1.IsEnabled = true;
            menuItem1.Foreground = new SolidColorBrush(Color.FromRgb(225, 225, 225));

            contextMenu.Items.Add(menuItem1);

            ////////////////////////////LISTBOX STYLE//////////////////////////////
            var listboxItemStyle = new Style(typeof(ListBoxItem));
            listboxItemStyle.Setters.Add(new Setter(PaddingProperty, new Thickness(0)));
            var listBoxContent = new ListBox
            {
                Background = new SolidColorBrush(Colors.Transparent) { Opacity = 1 }
            };
            listBoxContent.SetValue(Panel.IsItemsHostProperty, true);
            listBoxContent.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
            listBoxContent.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            //////////////////////////////////////////////////////////////////////

            var addDescription = true;
            // If the item contains effects, add them
            var validItem = item.ObjectEffectsToString.FirstOrDefault(i => i.Length > 2);
            if (item.ObjectEffectsToString?.Count > 0 && validItem != null)
            {
                addDescription = false;
                // Second Menu Item => Effects label

                var menuItem2 = new MenuItem
                {
                    Style = Application.Current.Resources["CustomMenuItem"] as Style,
                    Header = LanguageManager.Translate("682"),
                    FontWeight = FontWeights.DemiBold,
                    IsEnabled = false,
                    Foreground = new SolidColorBrush(Color.FromRgb(225, 225, 225))
                };

                contextMenu.Items.Add(menuItem2);

                // Third Menu Item => Effects content

                var menuItem3 = new MenuItem
                {
                    Style = Application.Current.Resources["CustomMenuItem"] as Style,
                    IsEnabled = false,
                    Foreground = new SolidColorBrush(Color.FromRgb(225, 225, 225))
                };

                foreach (var effect in item.ObjectEffectsToString)
                    if (effect.Length > 2)
                    {
                        var effectTextBlock = new TextBlock
                        {
                            Margin = new Thickness(0, -3, 0, 0),
                            FontWeight = FontWeights.SemiBold,
                            Padding = new Thickness(0, 0, 0, 0),
                            Text = effect
                        };

                        var maxIndex = effect.Length < 5 ? effect.Length : 5;
                        if (effect.Substring(0, maxIndex).Contains("-"))
                            effectTextBlock.Foreground = Brushes.PaleVioletRed;
                        else if (effect.Any(char.IsDigit))
                            effectTextBlock.Foreground = Brushes.LimeGreen;
                        else
                            effectTextBlock.Foreground = Brushes.White;

                        listBoxContent.Items.Add(effectTextBlock);
                    }

                listBoxContent.ItemContainerStyle = listboxItemStyle;

                menuItem3.Header = listBoxContent;
                contextMenu.Items.Add(menuItem3);
            }
            else if (item.SortedDropMonsterIds != null && item.SortedDropMonsterIds?.Count > 0) // it may be a resource
            {
                var menuItem4 = new MenuItem
                {
                    Style = Application.Current.Resources["CustomMenuItem"] as Style,
                    IsEnabled = true,
                    Foreground = new SolidColorBrush(Color.FromRgb(225, 225, 225))
                };

                var stackPanel = new StackPanel();
                stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                listBoxContent.ItemContainerStyle = listboxItemStyle;

                foreach (var monster in item.SortedDropMonsterIds.OrderBy(i => int.Parse(i)))
                {
                    var tbContainer = new TextBlock();
                    var hyperMonsterImg = new Hyperlink();
                    hyperMonsterImg.RequestNavigate += HyperMonsterImg_RequestNavigate;
                    if (monster != "7777")
                    {
                        hyperMonsterImg.ToolTip = LanguageManager.Translate("698");
                        var monsterImg = new Image {Width = 32, Height = 32};
                        hyperMonsterImg.NavigateUri =
                            new Uri($"https://www.dofus-touch.com/fr/mmorpg/encyclopedie/monstres/{monster}");
                        monsterImg.Source = new BitmapImage(new Uri(
                            $"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/gfx/monsters/{monster}.png"));
                        monsterImg.Margin = new Thickness(0, -3, 0, 0);

                        hyperMonsterImg.Inlines.Add(monsterImg);
                        tbContainer.Inlines.Add(hyperMonsterImg);
                    }
                    else
                    {
                        var toomanyImg = new Image {Width = 32, Height = 32};
                        toomanyImg.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/too_many.png"));
                        toomanyImg.Margin = new Thickness(0, -3, 0, 0);

                        hyperMonsterImg.Inlines.Add(toomanyImg);
                        tbContainer.Inlines.Add(hyperMonsterImg);
                    }

                    stackPanel.Children.Add(tbContainer);
                }

                listBoxContent.Items.Add(stackPanel);
                menuItem4.Header = listBoxContent;
                contextMenu.Items.Add(menuItem4);
            }

            // Footer menu item
            var menuItem5 = new MenuItem();
            menuItem5.Style = Application.Current.Resources["CustomMenuItem"] as Style;

            menuItem5.IsEnabled = false;
            menuItem5.Foreground = new SolidColorBrush(Color.FromRgb(225, 225, 225));
            menuItem5.Margin = new Thickness(-10, 8, 0, 10);

            var footer = new TextBlock();
            if (item.StringType != null && item.StringType != "??") // category
            {
                footer.Inlines.Add(new Run {Text = LanguageManager.Translate("683") + ": "});
                footer.Inlines.Add(new Run {FontWeight = FontWeights.DemiBold, Text = item.StringType});
                footer.Inlines.Add(new LineBreak());
            }

            // weight
            footer.Inlines.Add(new Run {Text = LanguageManager.Translate("696") + ": "});
            footer.Inlines.Add(new Run {FontWeight = FontWeights.DemiBold, Text = item.RealWeight + " pods"});
            footer.Inlines.Add(new LineBreak());
            // price
            footer.Inlines.Add(new Run {Text = LanguageManager.Translate("697") + ": "});
            footer.Inlines.Add(new Run {FontWeight = FontWeights.DemiBold, Text = item.Price + " K"});

            menuItem5.Header = footer;
            contextMenu.Items.Add(menuItem5);

            if (addDescription && item.Description?.Length > 0)
            {
                var menuItem6 = new MenuItem();
                menuItem6.Style = Application.Current.Resources["CustomMenuItem"] as Style;
                menuItem6.IsEnabled = false;
                menuItem6.FontSize = 12;
                menuItem6.Margin = new Thickness(-15, -5, 0, 15);
                menuItem6.FontStyle = FontStyles.Italic;

                if (item.Type != default && item.Type == ObjectTypes.CONSUMABLE)
                    menuItem6.Foreground = new SolidColorBrush(Colors.Orange);
                else
                    menuItem6.Foreground = new SolidColorBrush(Colors.DimGray);

                var description = new TextBlock();
                description.Width = 250;
                description.TextWrapping = TextWrapping.WrapWithOverflow;
                description.Text = item.Description;

                menuItem6.Header = description;
                contextMenu.Items.Add(menuItem6);
            }

            parent.ContextMenu = contextMenu;
            contextMenu.IsOpen = true;
            ContextMenus.Add(contextMenu);
            contextMenu.Closed += ContextMenu_Closed;
        }

        private void MenuItem1_MouseEnter(object sender, MouseEventArgs e)
        {
            var mi = sender as MenuItem;
            mi.Background = new SolidColorBrush(Colors.Black) {Opacity = 0.65};
            mi.UpdateLayout();
        }

        private void CloseBtn_Click(object sender, MouseButtonEventArgs e, ContextMenu cm)
        {
            if (cm != null && !cm.IsOpen)
            {
                cm.Closed -= ContextMenu_Closed;
                if (cm.Items?.Count > 0) cm.Items.Clear();
            }

            if (ContextMenus.Contains(cm))
                ContextMenus.Remove(cm);

            if (Mouse.OverrideCursor != Cursors.Hand)
                Mouse.OverrideCursor = Cursors.Hand;
        }

        private void CloseBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Mouse.OverrideCursor != Cursors.Arrow)
                Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void CloseBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Mouse.OverrideCursor != Cursors.Hand)
                Mouse.OverrideCursor = Cursors.Hand;
        }

        private void HyperMonsterImg_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (e.Uri.ToString() != "")
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            var cm = sender as ContextMenu;
            if (cm != null && !cm.IsOpen)
            {
                cm.Closed -= ContextMenu_Closed;
                if (cm.Items?.Count > 0) cm.Items.Clear();
            }

            if (ContextMenus.Contains(cm))
                ContextMenus.Remove(cm);
        }

        private void ObjectLog_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Mouse.OverrideCursor != Cursors.Arrow)
                Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void ObjectLog_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Mouse.OverrideCursor != Cursors.Hand)
                Mouse.OverrideCursor = Cursors.Hand;
        }

        private void BtnClearLogs_Click(object sender, RoutedEventArgs e)
        {
            Account.Logger.Logs.Clear();
        }

        private async void TxtCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (TxtCommand.Text.Length == 0 || e.Key != Key.Enter)
                return;

            await Account.Commands.HandleInput(TxtCommand.Text);
            TxtCommand.Clear();
        }

        private void BtnCopyLogs_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(string.Join(Environment.NewLine, Account.Logger.Logs.Select(o => o.ToString())));
        }

        private void BtnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            var status = (PlayerStatusEnum) Convert.ToInt32((sender as MenuItem).Tag);
            Account.Game.Character.ChangeStatus(status);
        }

        private void BtnReportBug_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofd = new SaveFileDialog();
                ofd.Filter = "Bubble Bug (.bbug) | *.bbug";
                ofd.FileName = $"BB_Bug_{DateTime.Now:dd-MM-yyyy_HH-mm}.bbug";

                var result = ofd.ShowDialog();
                if (result.HasValue && result.Value)
                    using (var bw = new BinaryWriter(File.Open(ofd.FileName, FileMode.Create)))
                    {
                        bw.Write(Account.Game.Character.Level);
                        bw.Write(Account.Game.Character.Inventory.WeightPercent);
                        bw.Write(Account.Game.Map.Id);
                        bw.Write(Account.Game.Map.CurrentPosition);

                        bw.Write(Account.Logger.Logs.Count);
                        foreach (var log in Account.Logger.Logs)
                            bw.Write(log.ToString());

                        bw.Write(Account.Network.Messages.Count);
                        foreach (var msg in Account.Network.Messages)
                        {
                            bw.Write(msg.Time.ToBinary());
                            bw.Write(msg.Sent);
                            bw.Write(msg.Message);
                        }
                    }
            }
            catch (Exception ex)
            {
                Account.Logger.LogError("", ex.ToString());
            }
        }
    }
}