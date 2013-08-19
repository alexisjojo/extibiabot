using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Cache;
using System.Text.RegularExpressions;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia
{
    /// <summary>
    /// Interaction logic for CbSpecialLoc.xaml
    /// </summary>
    public partial class CbSpecialLoc : Window
    {
        private SpecialArea _specialArea;

        public CbSpecialLoc()
        {
            InitializeComponent();
        }

        public CbSpecialLoc(SpecialArea specialArea)
        {
            InitializeComponent();

            _specialArea = specialArea;

            FillFields();
        }

        void FillFields()
        {

            AreaName.Text = _specialArea.Name;

            AreaLeft.Text = _specialArea.DimensionLeft.ToString();
            AreaTop.Text = _specialArea.DimensionTop.ToString();
            AreaRight.Text = _specialArea.DimensionRight.ToString();
            AreaDown.Text = _specialArea.DimensionDown.ToString();

            AreaConsideration.SelectedItem = (WalkSender)_specialArea.Consideration;

            SaImage.Source = _specialArea.Image;
        }

        public void UpdateImage()
        {
            try
            {
                Regex regex = new Regex("^\\d+$");

                if (!regex.IsMatch(AreaLeft.Text))
                    return;
                if (!regex.IsMatch(AreaTop.Text))
                    return;
                if (!regex.IsMatch(AreaRight.Text))
                    return;
                if (!regex.IsMatch(AreaDown.Text))
                    return;

                _specialArea.DimensionLeft = int.Parse(AreaLeft.Text);
                _specialArea.DimensionTop = int.Parse(AreaTop.Text);
                _specialArea.DimensionRight = int.Parse(AreaRight.Text);
                _specialArea.DimensionDown = int.Parse(AreaDown.Text);

                if (!HelpMethods.CheckValueInRange(0, 10, _specialArea.DimensionLeft))
                    _specialArea.DimensionLeft = 0;

                if (!HelpMethods.CheckValueInRange(0, 10, _specialArea.DimensionTop))
                    _specialArea.DimensionTop = 0;

                if (!HelpMethods.CheckValueInRange(0, 10, _specialArea.DimensionRight))
                    _specialArea.DimensionRight = 0;

                if (!HelpMethods.CheckValueInRange(0, 10, _specialArea.DimensionDown))
                    _specialArea.DimensionDown = 0;

                SaImage.Source = _specialArea.Image;               
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void AreaKeyUp(object sender, KeyEventArgs e)
        {
            UpdateImage();
        }

        private void SaSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SpecialArea specialArea = new SpecialArea(
                    AreaName.Text,
                    int.Parse(AreaLeft.Text),
                    int.Parse(AreaTop.Text),
                    int.Parse(AreaRight.Text),
                    int.Parse(AreaDown.Text),
                    (WalkSender)AreaConsideration.SelectedItem);

                specialArea.Active = true;

                if (specialArea.IsValid())
                {
                    if (_specialArea == null)
                        CaveBot.Instance.SpecialAreasList.Add(specialArea);
                    else
                    {
                        CaveBot.Instance.SpecialAreasList.Remove(_specialArea);
                        CaveBot.Instance.SpecialAreasList.Add(specialArea);
                    }
                }

            }
            catch
            {

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
