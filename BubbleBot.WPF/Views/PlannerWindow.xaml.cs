using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BubbleBot.Configurations;

namespace BubbleBot.Views.Planner
{
    public partial class PlannerWindow
    {
        // Constructor
        public PlannerWindow()
        {
            InitializeComponent();

            TempCollection = new ObservableCollection<bool>(Enumerable.Repeat(false, 24));
            TempForceScript = false;
            TempActivated = false;

            DataContext = GlobalConfiguration.Instance;
        }

        public ObservableCollection<bool> TempCollection { get; set; }
        public bool TempForceScript { get; set; }
        public bool TempActivated { get; set; }

        private void LbPlanification_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LbAccounts.SelectedItem == null || LbPlanification.SelectedIndex == -1)
                return;

            var index = LbPlanification.SelectedIndex;

            if (LbAccounts.SelectedItems != null && LbAccounts.SelectedItems.Count > 1)
            {
                TempCollection[index] = !TempCollection[index];
                return;
            }


            var account = LbAccounts.SelectedItem as AccountConfiguration;
            account.Planification[index] = !account.Planification[index];
            GlobalConfiguration.Instance.Save();
        }

        private void BtnSaveMultipleAccounts_OnClick(object sender, RoutedEventArgs e)
        {
            // Just a security
            if (LbAccounts.SelectedItems == null || LbAccounts.SelectedItems.Count <= 1)
                return;

            foreach (AccountConfiguration account in LbAccounts.SelectedItems)
            {
                if (PlanificationActivatedCheckbox.IsChecked == true)
                    account.PlanificationActivated = true;

                if (PlanificationForceScriptCheckbox.IsChecked == true)
                    account.ForceStartScript = true;

                for (var i = 0; i < 24; i++) account.Planification[i] = TempCollection[i];
            }

            GlobalConfiguration.Instance.Save();
        }

        private void LbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Binding tempActivated = new Binding("TempActivated");
            tempActivated.Source = this;
            Binding tempForceScript = new Binding("TempForceScript");
            tempForceScript.Source = this;
            
            if (LbAccounts.SelectedItems != null && LbAccounts.SelectedItems.Count > 1)
            {
                var acc = LbAccounts.SelectedItem as AccountConfiguration;
                LbPlanification.ItemsSource = TempCollection;
                BindingOperations.SetBinding(PlanificationActivatedCheckbox, CheckBox.IsCheckedProperty, tempActivated);
                BindingOperations.SetBinding(PlanificationForceScriptCheckbox, CheckBox.IsCheckedProperty, tempForceScript);
                BtnSaveMultipleAccounts.Visibility = Visibility.Visible;
                return;
            }

            if (BtnSaveMultipleAccounts.Visibility == Visibility.Visible)
                BtnSaveMultipleAccounts.Visibility = Visibility.Hidden;

            if (LbAccounts.SelectedItems != null && LbAccounts.SelectedItems.Count == 0)
            {
                LbPlanification.ItemsSource = null;
                BindingOperations.SetBinding(PlanificationActivatedCheckbox, CheckBox.IsCheckedProperty, tempActivated);
                BindingOperations.SetBinding(PlanificationForceScriptCheckbox, CheckBox.IsCheckedProperty, tempForceScript);
                return;
            }

            var account = LbAccounts.SelectedItem as AccountConfiguration;

            Binding planificationActivated = new Binding("PlanificationActivated");
            planificationActivated.Source = account;
            Binding forceStartScript = new Binding("ForceStartScript");
            forceStartScript.Source = account;

            LbPlanification.ItemsSource = account.Planification;
            BindingOperations.SetBinding(PlanificationActivatedCheckbox, CheckBox.IsCheckedProperty, planificationActivated);
            BindingOperations.SetBinding(PlanificationForceScriptCheckbox, CheckBox.IsCheckedProperty, forceStartScript);

            TempCollection = new ObservableCollection<bool>(Enumerable.Repeat(false, 24));
            TempActivated = false;
            TempForceScript = false;
        }

        private void BtnSelectAll_OnClick(object sender, RoutedEventArgs e)
        {
            LbAccounts.SelectAll();
        }

        private void BtnUnselectAll_OnClick(object sender, RoutedEventArgs e)
        {
            if (LbAccounts.SelectedItems == null)
                return;

            LbAccounts.UnselectAll();
        }
    }
}