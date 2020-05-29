using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BubbleBot.Configurations;
using BubbleBot.Core.Accounts;
using BubbleBot.Core.Accounts.InGame.Managers.Movements;
using BubbleBot.Core.Pathfinding;
using BubbleBot.Utility.DofusTouch;
using BubbleBot.Utility.Extensions;
using BubbleBot.Views.Accounts.MapViewer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using Image = System.Drawing.Image;
using System.ComponentModel;
using BubbleBot.Core.Accounts.InGame.Map.Entities;
using System.Threading;

namespace BubbleBot.Views.Accounts
{
    public partial class MapViewerUc : INotifyPropertyChanged
    {
        // Fields
        private static List<MapViewerCell> _cellsPoints;
        private static Pen _pen;
        private static Brush _walkableCellBrush;
        private static Brush _losCellBrush;
        private static Brush _obstacleCellBrush;
        private static Brush _selectedCellBrush;
        private static Brush _ourPlayerBrush;
        private static Brush _monstersGroupsBrush;
        private static Brush _playersBrush;
        private static Brush _doorsBrush;
        private static Brush _interactivesBrush;
        private static Brush _npcsBrush;
        private static BitmapImage _sunImage;
        private static BitmapImage _phenixImage;
        private static BitmapImage _lockedStorageImage;
        private List<short> _path;
        private short _selectedCellId;
        private bool _showCellIds;
        private bool _showRealMap;
        private bool _showNames;
        private bool _showIds;
        private bool _showMonsters;

        // Constructor
        public MapViewerUc()
        {
            InitializeComponent();
            Initialize();

            DataContextChanged += MapViewerUc_DataContextChanged;
            MouseLeftButtonUp += MapViewerUc_MouseLeftButtonUp;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        // Properties
        public bool ShowCellIds
        {
            get => _showCellIds;
            set
            {
                _showCellIds = value;
                OnPropertyChanged("ShowCellsIds");
                InvalidateVisual();
            }
        }

        public bool ShowNames
        {
            get => _showNames;
            set
            {
                if (ShowIds) ShowIds = false;
                _showNames = value;
                OnPropertyChanged("ShowNames");
                InvalidateVisual();
            }
        }

        public bool ShowIds
        {
            get => _showIds;
            set
            {
                if (ShowNames) ShowNames = false;
                _showIds = value;
                OnPropertyChanged("ShowIds");
                InvalidateVisual();
            }
        }

        public bool ShowMonsters
        {
            get => _showMonsters;
            set
            {
                _showMonsters = value;
                OnPropertyChanged("ShowMonsters");
                InvalidateVisual();
            }
        }

        public bool ShowRealMap
        {
            get => _showRealMap;
            set
            {
                _showRealMap = value;
                InvalidateVisual();

                if (!_showRealMap && RealMap.ImageSource != null)
                {
                    RealMap.Dispatcher.Invoke(() => RealMap.ImageSource = null);
                    _currentRealMapId = 0;
                }
                else if (_showRealMap)
                {
                    GenerateRealMap();
                }

            }
        }

        private Account Account => BubbleBotMain.Instance.SelectedAccount;
        private bool IsMapValid => Account?.Game?.Map?.Data != null;
        private static ImageSource _realMap;
        private static int _currentRealMapId;

        private static void Initialize()
        {
            if (_cellsPoints != null)
                return;

            _currentRealMapId = 0;
            _pen = new Pen(Brushes.White, 1);
            _walkableCellBrush = new SolidColorBrush(Colors.DarkGray);
            _losCellBrush = new SolidColorBrush(Colors.Transparent);
            _obstacleCellBrush = new SolidColorBrush(Colors.Gray);
            _selectedCellBrush = new SolidColorBrush(Color.FromArgb(255, 91, 90, 90));
            _ourPlayerBrush = new SolidColorBrush(Colors.Blue);
            _monstersGroupsBrush = new SolidColorBrush(Color.FromArgb(255, 139, 0, 0));
            _playersBrush = new SolidColorBrush(Color.FromArgb(255, 81, 113, 202));
            _doorsBrush = new SolidColorBrush(Color.FromArgb(255, 150, 75, 13));
            _interactivesBrush = new SolidColorBrush(Color.FromArgb(255, 1, 143, 140));
            _npcsBrush = new SolidColorBrush(Color.FromArgb(255, 179, 120, 211));
            _sunImage = new BitmapImage(new Uri("pack://application:,,,/Resources/21000.png"));
            _phenixImage = new BitmapImage(new Uri("pack://application:,,,/Resources/7521.png"));
            _lockedStorageImage = new BitmapImage(new Uri("pack://application:,,,/Resources/12367.png"));

            // Freeze the used objects
            _pen.Freeze();
            _walkableCellBrush.Freeze();
            _losCellBrush.Freeze();
            _obstacleCellBrush.Freeze();
            _selectedCellBrush.Freeze();
            _ourPlayerBrush.Freeze();
            _monstersGroupsBrush.Freeze();
            _playersBrush.Freeze();
            _doorsBrush.Freeze();
            _interactivesBrush.Freeze();
            _npcsBrush.Freeze();
            _sunImage.Freeze();
            _phenixImage.Freeze();
            _lockedStorageImage.Freeze();

            _cellsPoints = new List<MapViewerCell>(560);
            short cell = 0;

            for (var i = 0; i < DTConstants.MAP_HEIGHT; i++)
            for (var j = 0; j < DTConstants.MAP_WIDTH * 2; j++)
            {
                if (cell.TryGetCoord(out var x, out var y))
                {
                    var startPtX = x * DTConstants.TileWidth + (y % 2 == 1 ? DTConstants.TileWidth / 2 : 0);
                    var startPtY = y * DTConstants.TileHeight / 2;
                    _cellsPoints.Add(new MapViewerCell(new[]
                    {
                        new Point(startPtX + DTConstants.TileWidth / 2, startPtY),
                        new Point(startPtX + DTConstants.TileWidth, startPtY + DTConstants.TileHeight / 2),
                        new Point(startPtX + DTConstants.TileWidth / 2, startPtY + DTConstants.TileHeight),
                        new Point(startPtX, startPtY + DTConstants.TileHeight / 2)
                    }));
                }

                cell++;
            }

        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            for (short i = 0; i < _cellsPoints.Count; i++)
            {

                var brush = GetCellBrush(i);

                if (brush == _obstacleCellBrush && !ShowCellIds && !ShowRealMap)
                {
                    _cellsPoints[i].DrawObstacle(drawingContext, brush, _pen);
                }
                else
                {
                    if (!(_showRealMap && brush == _losCellBrush) && !(_showRealMap && brush == _obstacleCellBrush))
                        _cellsPoints[i].Draw(drawingContext, brush, _pen, ShowRealMap);


                    if (_path?.Contains(i) == true) _cellsPoints[i].DrawCross(drawingContext, _pen);
                }

                if (ShowCellIds)
                {
                    var fText = new FormattedText(i.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        new Typeface("Segoe UI"), 10, brush == _losCellBrush ? Brushes.White : Brushes.Black,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    drawingContext.DrawText(fText,
                        new Point(_cellsPoints[i].Points[0].X - fText.Width / 2,
                            _cellsPoints[i].Points[1].Y - fText.Height / 2));
                }

                if (ShowNames)
                {
                    FormattedText fText = null;
                    var player = Account.Game.Map.Players.FirstOrDefault(p => p.CellId == i);
                    
                    if (player != null)
                    {
                        fText = new FormattedText($"{player.Name} (Lvl {player.Level})", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        new Typeface("Segoe UI"), 10, brush == _losCellBrush ? Brushes.White : _playersBrush,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    }
                    else if (Account.Game.Map.PlayedCharacter?.CellId == i)
                    {
                        fText = new FormattedText($"{Account.Game.Map.PlayedCharacter.Name} (Lvl {Account.Game.Map.PlayedCharacter.Level})", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        new Typeface("Segoe UI"), 10, brush == _losCellBrush ? Brushes.White : _ourPlayerBrush,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    }
                    else if (Account.Game.Map.Npcs.FirstOrDefault(n => n.CellId == i) != null)
                    {
                        var npc = Account.Game.Map.Npcs.FirstOrDefault(n => n.CellId == i);

                        fText = new FormattedText($"{npc.Name}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        new Typeface("Segoe UI"), 10, brush == _losCellBrush ? Brushes.White : _npcsBrush,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    }

                    if (fText != null)
                    {
                        fText.SetFontWeight(FontWeights.Bold);

                        drawingContext.DrawText(fText,
                        new Point(_cellsPoints[i].Points[0].X - fText.Width / 2,
                                  _cellsPoints[i].Points[1].Y - fText.Height * 2));
                    }
                }

                if (ShowIds)
                {
                    FormattedText fText = null;
                    var player = Account.Game.Map.Players.FirstOrDefault(p => p.CellId == i);

                    if (player != null)
                    {
                        fText = new FormattedText($"{player.Id}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        new Typeface("Segoe UI"), 10, brush == _losCellBrush ? Brushes.White : _playersBrush,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    }
                    else if (Account.Game.Map.PlayedCharacter?.CellId == i)
                    {
                        fText = new FormattedText($"{Account.Game.Map.PlayedCharacter.Id}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        new Typeface("Segoe UI"), 10, brush == _losCellBrush ? Brushes.White : _ourPlayerBrush,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    }
                    else if (Account.Game.Map.Npcs.FirstOrDefault(n => n.CellId == i) != null)
                    {
                        var npc = Account.Game.Map.Npcs.FirstOrDefault(n => n.CellId == i);

                        fText = new FormattedText($"{npc.NpcId}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                        new Typeface("Segoe UI"), 10, brush == _losCellBrush ? Brushes.White : _npcsBrush,
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);
                    }

                    if (fText != null)
                    {
                        fText.SetFontWeight(FontWeights.Bold);

                        drawingContext.DrawText(fText,
                        new Point(_cellsPoints[i].Points[0].X - fText.Width / 2,
                                  _cellsPoints[i].Points[1].Y - fText.Height * 2));
                    }
                }

                if (ShowMonsters)
                {
                    var monsterGroup = Account.Game.Map.MonstersGroups.FirstOrDefault(mg => mg.CellId == i);
                    if (monsterGroup != null)
                    {
                        string monsterInfos = null;
                        List<MonsterEntry> monsters = new List<MonsterEntry>();
                        monsters.Add(monsterGroup.Leader);
                        monsters.AddRange(monsterGroup.Followers);
                        foreach (var monster in monsters)
                        {
                            if (monster != monsters.Last())
                                monsterInfos += $"{monster.Name} ({monster.Level})\n";
                            else
                                monsterInfos += $"{monster.Name} ({monster.Level})";
                        }

                        string newTotalXp = String.Format("{0:n0}", monsterGroup.TotalXp).Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, " ");
                        var fText = new FormattedText($"Rang {monsterGroup.TotalLevel}\n{newTotalXp} XP\n{monsterInfos}", CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                            new Typeface("Arial"), 10, brush == _losCellBrush ? Brushes.White : _monstersGroupsBrush,
                            VisualTreeHelper.GetDpi(this).PixelsPerDip);

                        fText.TextAlignment = TextAlignment.Center;
                        fText.SetFontWeight(FontWeights.Bold);

                        drawingContext.DrawText(fText,
                        new Point(_cellsPoints[i].Points[0].X,
                                  _cellsPoints[i].Points[1].Y - fText.Height * 1.2));
                    }
                }

                if (IsMapValid)
                {
                    // Draw the sun image if this cell has it
                    if (Account.Game.Map.TeleportableCells.Contains(i))
                        _cellsPoints[i].DrawImage(drawingContext, _sunImage);
                    else if (Account.Game.Map.Phenixs.FirstOrDefault(p => p.CellId == i) != null)
                        _cellsPoints[i].DrawImage(drawingContext, _phenixImage);
                    else if (Account.Game.Map.LockedStorages.FirstOrDefault(ls => ls.CellId == i) != null)
                        _cellsPoints[i].DrawImage(drawingContext, _lockedStorageImage);

                    DrawTileContent(drawingContext, i);
                }
            }
        }

        private Brush GetCellBrush(short cell)
        {
            // In case the cell is currently selected
            if (cell == _selectedCellId)
                return _selectedCellBrush;

            // In case the cell is a possible placement
            if (IsMapValid && Account.IsFighting() &&
                Account.Game.Fight.PositionsForChallengers?.Contains(cell) == true)
                return Brushes.Red;

            if (IsMapValid && Account.IsFighting() && Account.Game.Fight.PositionsForDefenders?.Contains(cell) == true)
                return Brushes.Blue;

            var brush = _losCellBrush;

            if (IsMapValid && Account.Game.Map.Data.Cells[cell].IsObstacle())
                brush = _obstacleCellBrush;
            else if (IsMapValid && Account.Game.Map.Data.Cells[cell].IsWalkable(Account.IsFighting()))
                brush = _walkableCellBrush;

            return brush;
        }

        private void DrawTileContent(DrawingContext drawingContext, short cellId)
        {
            if (Account.IsFighting())
            {
                if (Account.Game.Fight.PlayedFighter?.CellId == cellId)
                    _cellsPoints[cellId].DrawPie(drawingContext, _ourPlayerBrush);
                else if (Account.Game.Fight.Allies.FirstOrDefault(a => a.CellId == cellId) != null)
                    _cellsPoints[cellId].DrawPie(drawingContext, _playersBrush);
                else if (Account.Game.Fight.Ennemies.FirstOrDefault(e => e.CellId == cellId) != null)
                    _cellsPoints[cellId].DrawPie(drawingContext, _monstersGroupsBrush);
            }
            else
            {
                if (Account.Game.Map.PlayedCharacter?.CellId == cellId)
                    _cellsPoints[cellId].DrawPie(drawingContext, _ourPlayerBrush);
                else if (Account.Game.Map.MonstersGroups.FirstOrDefault(mg => mg.CellId == cellId) != null)
                    _cellsPoints[cellId].DrawPie(drawingContext, _monstersGroupsBrush);
                else if (Account.Game.Map.Players.FirstOrDefault(p => p.CellId == cellId) != null)
                    _cellsPoints[cellId].DrawPie(drawingContext, _playersBrush);
                else if (Account.Game.Map.Doors.FirstOrDefault(d => d.CellId == cellId) != null)
                    _cellsPoints[cellId].DrawRectangle(drawingContext, _doorsBrush);
                else if (Account.Game.Map.StatedElements.FirstOrDefault(se => se.CellId == cellId) != null ||
                         Account.Game.Map.Zaap?.CellId == cellId || Account.Game.Map.Zaapi?.CellId == cellId)
                    _cellsPoints[cellId].DrawRectangle(drawingContext, _interactivesBrush);
                else if (Account.Game.Map.Npcs.FirstOrDefault(n => n.CellId == cellId) != null)
                    _cellsPoints[cellId].DrawPie(drawingContext, _npcsBrush);
            }
        }

        private void MapViewerUc_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // No need to check if the map is not valid or the bot is not inactif
            if (!IsMapValid || Account.IsBusy)
                return;

            var pos = e.GetPosition(this);

            for (short i = 0; i < _cellsPoints.Count; i++)
                if (_cellsPoints[i].IsPointInside(pos))
                {
                    if (Account.Game.Map.Data.Cells[i].IsWalkable(false))
                    {
                        _selectedCellId = i;
                        InvalidateVisual();

                        Task.Run(async () =>
                        {
                            await Task.Delay(200);

                            if (_selectedCellId != -1)
                            {
                                _selectedCellId = -1;
                                Application.Current.Dispatcher.Invoke(InvalidateVisual);
                            }
                        });

                        HandleWalkableCellClicked(i);
                    }

                    break;
                }
        }

        private void HandleWalkableCellClicked(short cell)
        {
            // Check if we can change the map from this cell
            if (Account.Game.Managers.Movements.CanChangeMap(cell, MapChangeDirections.LEFT))
                Account.Game.Managers.Movements.ChangeMap(MapChangeDirections.LEFT, cell);
            else if (Account.Game.Managers.Movements.CanChangeMap(cell, MapChangeDirections.RIGHT))
                Account.Game.Managers.Movements.ChangeMap(MapChangeDirections.RIGHT, cell);
            else if (Account.Game.Managers.Movements.CanChangeMap(cell, MapChangeDirections.TOP))
                Account.Game.Managers.Movements.ChangeMap(MapChangeDirections.TOP, cell);
            else if (Account.Game.Managers.Movements.CanChangeMap(cell, MapChangeDirections.BOTTOM))
                Account.Game.Managers.Movements.ChangeMap(MapChangeDirections.BOTTOM, cell);
            // Otherwise just move to the cell
            else
                Console.WriteLine(Account.Game.Managers.Movements.MoveToCell(cell));
        }

        #region GenerateRealMap

        private object bufferLock = new object();
       
        public static List<Task> TaskList = new List<Task>();
        private static bool running = false;
        private void GenerateRealMap()
        {
            if (!running)
            {
                running = true;
                if (Account.Game?.Map?.Id == (default) || Account.Game?.Map?.Id == 0)
                {
                    running = false;
                    return;
                }

                if (Account.Game?.Map?.Id == _currentRealMapId)
                {
                    running = false;
                    return;
                }

                var mapid = Account.Game.Map.Id;

                string jsonMap = null;
                WebClient wc = new WebClient();
                SetProxy(wc);

                jsonMap = wc.DownloadString($"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/maps/{mapid}.json");
                if (jsonMap == null)
                {
                    wc?.Dispose();
                    running = false;
                    return;
                }

                byte[] bytes = wc.DownloadData($"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/backgrounds/{mapid}.jpg");

                if (bytes == null)
                {
                    wc?.Dispose();
                    running = false;
                    return;
                }

                MemoryStream ms = new MemoryStream(bytes);
                Image background = Image.FromStream(ms);
                //ms?.Dispose();

                JsonMap content = JsonConvert.DeserializeObject<JsonMap>(jsonMap);
                var canvas = Graphics.FromImage(background);

                var midgroundLayer = content.MidgroundLayer.Keys;
                foreach (var key in midgroundLayer)
                {
                    var task = Task.Run(() =>
                    {
                        if (content.MidgroundLayer[key] != null)
                        {
                            foreach (var element in content.MidgroundLayer[key])
                            {
                                if (element.G != null)
                                {
                                    if (element.Sx != null && element.Sy != null)
                                    {
                                        DownloadAsset(element.G, element.X * -1 - 58, element.Y * -1 - 15, element.Sx, element.Sy, element.Hue, background);
                                    }
                                    else if (element.Sx != null && element.Sy == null)
                                    {
                                        DownloadAsset(element.G, element.X * -1 - 58, element.Y + 15, element.Sx, 1, element.Hue, background);
                                    }
                                    else if (element.Sx == null && element.Sy != null)
                                    {
                                        DownloadAsset(element.G, element.X + 58, element.Y * -1 - 15, 1, element.Sy, element.Hue, background);
                                    }
                                    else DownloadAsset(element.G, element.X + 58, element.Y + 15, 1, 1, element.Hue, background);

                                }
                            }
                        }
                    });
                    TaskList.Add(task);
                }

                if (content.Foreground != null)
                {
                    bytes = wc.DownloadData($"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/foregrounds/{mapid}.png");

                    if (bytes == null)
                    {
                        canvas?.Dispose();
                        wc?.Dispose();
                        running = false;
                        return;
                    }

                    ms = new MemoryStream(bytes);
                    Image foreground = Image.FromStream(ms);
                    //ms?.Dispose();

                    lock (bufferLock)
                    {
                        canvas.DrawImage(foreground, 0, 0, (float)background.Width, (float)background.Height);
                    }

                    foreground?.Dispose();
                }

                Task.WaitAll(TaskList.ToArray());

                foreach (var task in TaskList)
                    if (task.IsCompleted || task.IsCanceled || task.IsFaulted)
                        task.Dispose();

                TaskList.Clear();

                _realMap = ToImageSource(background, ImageFormat.Png);
                _currentRealMapId = Account.Game.Map.Id;
                _realMap.Freeze();

                RealMap.Dispatcher.Invoke(() =>
                {
                    RealMap.ImageSource = _realMap;
                });

                background?.Dispose();
                canvas?.Dispose();
                wc?.Dispose();
                running = false;
            }
        }        

        private void DownloadAsset(long? asset, float x, float y, float? sx, float? sy, List<long> hue, System.Drawing.Image background)
        {

            if (asset == null)
                return;

            Image img;
            using (WebClient wc = new WebClient())
            {
                SetProxy(wc);
                byte[] bytes = wc.DownloadData($"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/gfx/world/png/{asset}.png");
                if (bytes == null)
                {
                    wc?.Dispose();
                    return;
                }

                MemoryStream ms = new MemoryStream(bytes);
                img = Image.FromStream(ms);
                //ms?.Dispose();
            }

            lock (bufferLock)
            {
                var ctx = Graphics.FromImage(background);
                ctx.ScaleTransform((float)sx, (float)sy);
                if (hue[0] == -128 && hue[1] == -128 && hue[2] == -128)
                {
                    ctx.RotateTransform(0);
                    img = AdjustBrightness(img, 0);
                }
                ctx.ResetTransform();
                ctx.DrawImage(img, x, y, img.Width, img.Height);
                img?.Dispose();
                ctx?.Dispose();
            }
        }

        public static ImageSource ToImageSource(Image image, ImageFormat imageFormat)
        {
            BitmapImage bitmap = new BitmapImage();

            using (MemoryStream stream = new MemoryStream())
            {
                // Save to the stream
                image.Save(stream, imageFormat);

                // Rewind the stream
                stream.Seek(0, SeekOrigin.Begin);

                // Tell the WPF BitmapImage to use this stream
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }

            return bitmap;
        }

        private Image AdjustBrightness(Image image, float brightness)
        {
            // Make the ColorMatrix.
            float b = brightness;
            ColorMatrix cm = new ColorMatrix(new float[][]
                {
            new float[] {b, 0, 0, 0, 0},
            new float[] {0, b, 0, 0, 0},
            new float[] {0, 0, b, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1},
                });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(cm);

            // Draw the image onto the new bitmap while applying
            // the new ColorMatrix.
            System.Drawing.Point[] points =
                {
            new System.Drawing.Point(0, 0),
            new System.Drawing.Point(image.Width, 0),
            new System.Drawing.Point(0, image.Height),
            };
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            // Make the result bitmap.
            Bitmap bm = new Bitmap(image.Width, image.Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.DrawImage(image, points, rect,
                    GraphicsUnit.Pixel, attributes);
            }

            // Return the result.
            return bm;
        }
        private void SetProxy(WebClient wc)
        {
            if (Account.AccountConfig.Proxy.IsValid)
            {
                if (Account.AccountConfig.Proxy.Ip != "" && Account.AccountConfig.Proxy.Port != 0)
                {
                    wc.Proxy = new WebProxy(Account.AccountConfig.Proxy.Ip, Account.AccountConfig.Proxy.Port);

                    if (Account.AccountConfig.Proxy.Username != "" && Account.AccountConfig.Proxy.Password != "")
                        wc.Proxy.Credentials = new NetworkCredential(Account.AccountConfig.Proxy.Username, Account.AccountConfig.Proxy.Password);
                }
            }
            else if (GlobalConfiguration.Instance.IsProxyValid)
            {
                if (GlobalConfiguration.Instance.ProxyIp != "" && GlobalConfiguration.Instance.ProxyPort != 0)
                {
                    wc.Proxy = new WebProxy(GlobalConfiguration.Instance.ProxyIp, GlobalConfiguration.Instance.ProxyPort);

                    if (GlobalConfiguration.Instance.ProxyUsername != "" && GlobalConfiguration.Instance.ProxyPassword != "")
                        wc.Proxy.Credentials = new NetworkCredential(GlobalConfiguration.Instance.ProxyUsername, GlobalConfiguration.Instance.ProxyPassword);
                }
            }
        }

        #endregion

        #region Invalidation

        private void MapViewerUc_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldAccount = e.OldValue as Account;
            var newAccount = e.NewValue as Account;

            if (oldAccount != null)
            {
                oldAccount.Game.Map.MapChanged -= RefreshMapViewer;
                oldAccount.Game.Map.EntitiesUpdated -= RefreshMapViewer;
                oldAccount.Game.Map.InteractivesUpdated -= RefreshMapViewer;
                oldAccount.Game.Map.PlayedCharacterMoving -= PlayedCharacterMoving;
                oldAccount.Game.Fight.FightJoined -= RefreshMapViewer;
                oldAccount.Game.Fight.PossiblePositionsReceived -= RefreshMapViewer;
                oldAccount.Game.Fight.FightStarted -= RefreshMapViewer;
                oldAccount.Game.Fight.FightersUpdated -= RefreshMapViewer;
                oldAccount.Game.Fight.PlayedFighterMoving -= PlayedCharacterMoving;
            }

            _selectedCellId = -1;
            _path = null;

            if (newAccount != null)
            {
                newAccount.Game.Map.MapChanged += RefreshMapViewer;
                newAccount.Game.Map.EntitiesUpdated += RefreshMapViewer;
                newAccount.Game.Map.InteractivesUpdated += RefreshMapViewer;
                newAccount.Game.Map.PlayedCharacterMoving += PlayedCharacterMoving;
                newAccount.Game.Fight.FightJoined += RefreshMapViewer;
                newAccount.Game.Fight.PossiblePositionsReceived += RefreshMapViewer;
                newAccount.Game.Fight.FightStarted += RefreshMapViewer;
                newAccount.Game.Fight.FightersUpdated += RefreshMapViewer;
                newAccount.Game.Fight.PlayedFighterMoving += PlayedCharacterMoving;
                InvalidateVisual();
            }
        }

        private void RefreshMapViewer()
        {
            if (_showRealMap && IsMapValid && Account.Game?.Map?.Id != _currentRealMapId)
            {
                GenerateRealMap();
            }
            else if (!_showRealMap && _currentRealMapId != 0)
            {
                _currentRealMapId = 0;
            }

            Application.Current?.Dispatcher.Invoke(() => InvalidateVisual());
        }

        private void PlayedCharacterMoving(List<short> path)
        {
            _path = path;
            Application.Current.Dispatcher.Invoke(() => InvalidateVisual());

            Task.Run(async () =>
            {
                await Task.Delay(PathDuration.Calculate(path));

                if (_path != null)
                {
                    _path = null;
                    Application.Current.Dispatcher.Invoke(() => InvalidateVisual());
                }
            });
        }

        #endregion
    }
}