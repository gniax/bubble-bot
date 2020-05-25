using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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

namespace BubbleBot.Views.Accounts
{
    public partial class AccountMapViewerUc : UserControl
    {
        public AccountMapViewerUc()
        {
            InitializeComponent();
        }
    }
}