using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RegX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Regex TheRegex;
        Stopwatch Timer;

        public MainWindow()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e) => Close()));

            Timer = new Stopwatch();
        }

        RegexOptions RegExOptions
        {
            get
            {
                RegexOptions regExOptions = RegexOptions.None;

                if ((bool)Compiled.IsChecked && Compiled.IsEnabled) regExOptions |= RegexOptions.Compiled;

                if ((bool)CultureInvariant.IsChecked && CultureInvariant.IsEnabled) regExOptions |= RegexOptions.CultureInvariant;

                if ((bool)ECMAScript.IsChecked && ECMAScript.IsEnabled) regExOptions |= RegexOptions.ECMAScript;

                if ((bool)ExplicitCapture.IsChecked && ExplicitCapture.IsEnabled) regExOptions |= RegexOptions.ExplicitCapture;

                if ((bool)IgnoreCase.IsChecked && IgnoreCase.IsEnabled) regExOptions |= RegexOptions.IgnoreCase;

                if ((bool)IgnorePatternWhitespace.IsChecked && IgnorePatternWhitespace.IsEnabled) regExOptions |= RegexOptions.IgnorePatternWhitespace;

                if ((bool)Multiline.IsChecked && Multiline.IsEnabled) regExOptions |= RegexOptions.Multiline;

                if ((bool)RightToLeft.IsChecked && RightToLeft.IsEnabled) regExOptions |= RegexOptions.RightToLeft;

                if ((bool)SingleLine.IsChecked && SingleLine.IsEnabled) regExOptions |= RegexOptions.Singleline;

                return regExOptions;
            }
        }

        void Replace(object sender = null, RoutedEventArgs e = null)
        {
            if (Query.Text == string.Empty || DataToQuery.Text == string.Empty) return;

            try
            {
                TheRegex = new Regex(Query.Text, RegExOptions);
                int NoOfMatches = TheRegex.Matches(DataToQuery.Text).Count;

                if (NoOfMatches == 0)
                {
                    Status.Content = "No Matches to Replace";
                    return;
                }

                DataToQuery.Text = TheRegex.Replace(DataToQuery.Text, ReplaceWith.Text);

                Status.Content = (NoOfMatches == 1) ? "Replaced 1 Match" : ("Replaced " + NoOfMatches + " Matches");
            }
            catch (Exception E) { Status.Content = E.Message; }
        }

        bool IsMatched
        {
            set
            {
                if (value)
                {
                    Matched.Content = "✔";
                    Matched.Foreground = new SolidColorBrush(Colors.Green);
                    Previous.IsEnabled = Next.IsEnabled = true;
                }
                else
                {
                    Matched.Content = "✖";
                    Status.Content = "No Matches";
                    Matched.Foreground = new SolidColorBrush(Colors.Red);
                    Previous.IsEnabled = Next.IsEnabled = false;
                }
            }
        }

        void Reset(object sender = null, RoutedEventArgs e = null)
        {
            IsMatched = false;
            QueryTime.Content = "0 ms";
            Results.Items.Clear();

            Query.Text = DataToQuery.Text = ReplaceWith.Text = string.Empty;

            Compiled.IsEnabled = RightToLeft.IsEnabled = CultureInvariant.IsEnabled = ExplicitCapture.IsEnabled = IgnorePatternWhitespace.IsEnabled = SingleLine.IsEnabled = true;
            Compiled.IsChecked = RightToLeft.IsChecked = CultureInvariant.IsChecked = ExplicitCapture.IsChecked = IgnorePatternWhitespace.IsChecked = SingleLine.IsChecked = false;

            ECMAScript.IsEnabled = Multiline.IsEnabled = IgnoreCase.IsEnabled = true;
            ECMAScript.IsChecked = Multiline.IsChecked = IgnoreCase.IsChecked = false;

            Status.Content = "Ready";
        }

        void Group()
        {
            GroupTree.Items.Clear();
            Results.Visibility = Visibility.Collapsed;
            GroupTree.Visibility = Visibility.Visible;

            if (Query.Text == string.Empty || DataToQuery.Text == string.Empty) return;
            try
            {
                Regex theExpr = new Regex(Query.Text, RegExOptions);
                MatchCollection C = theExpr.Matches(DataToQuery.Text);

                foreach (Match match in C)
                {
                    var Header = new TreeViewItem();
                    Header.Header = match.Value;
                    foreach (string groupName in theExpr.GetGroupNames())
                    {
                        Group theGroup = match.Groups[groupName];
                        if (groupName != "0")
                        {
                            var GroupHeader = new TreeViewItem();
                            GroupHeader.Header = "<" + groupName + "> " + theGroup;
                            Header.Items.Add(GroupHeader);
                            foreach (Capture theCapture in theGroup.Captures) GroupHeader.Items.Add(theCapture.Value);
                        }
                    }
                    GroupTree.Items.Add(Header);
                    Header.ExpandSubtree();
                }

                Status.Content = C.Count == 0 ? "No Matches" : "Matched";
            }
            catch (Exception E) { Status.Content = E.Message; }
        }

        void Match(object sender = null, RoutedEventArgs e = null)
        {
            IsMatched = false;

            if ((bool)ShowGroups.IsChecked)
            {
                Group();
                return;
            }
            else
            {
                Results.Items.Clear();
                GroupTree.Visibility = Visibility.Collapsed;
                Results.Visibility = Visibility.Visible;
            }

            if (Query.Text == string.Empty || DataToQuery.Text == string.Empty) return;

            try { TheRegex = new Regex(Query.Text, RegExOptions); }
            catch (Exception E)
            {
                Results.Items.Clear();
                Status.Content = E.Message;
                return;
            }

            // Clear Cache if Specified
            if (ClearCache.IsChecked.Value) Regex.CacheSize = 0;

            Timer.Start();
            MatchCollection C = TheRegex.Matches(DataToQuery.Text);

            if (C.Count == 0) IsMatched = false;
            else
            {
                IsMatched = true;
                Status.Content = (C.Count == 1) ? "1 Match Found" : (C.Count + " Matches Found");

                foreach (Match M in C)
                    Results.Items.Add(new MatchLabel(M.Index, M.Length, M.Value));
            }

            QueryTime.Content = Timer.ElapsedMilliseconds.ToString() + " ms";
            Timer.Reset();
        }

        /// <summary>
        /// Enabling ECMA Script disables many other Options.
        /// </summary>
        void EnableECMAScript(object sender, RoutedEventArgs e)
        {
            Compiled.IsEnabled = RightToLeft.IsEnabled = CultureInvariant.IsEnabled = ExplicitCapture.IsEnabled = IgnorePatternWhitespace.IsEnabled = SingleLine.IsEnabled = false;
        }

        void DisableECMAScript(object sender, RoutedEventArgs e)
        {
            Compiled.IsEnabled = RightToLeft.IsEnabled = CultureInvariant.IsEnabled = ExplicitCapture.IsEnabled = IgnorePatternWhitespace.IsEnabled = SingleLine.IsEnabled = true;
        }

        void QueryChanged(object sender, TextChangedEventArgs e) { if ((bool)Async.IsChecked) Match(); }

        void Query_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) Match(); }
        
        void Results_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MatchLabel ML = ((MatchLabel)Results.SelectedItem);
                DataToQuery.Focus();
                DataToQuery.Select(ML.Index, ML.Length);
            }
            catch { }
        }

        void DataToQuery_Drop(object sender, DragEventArgs e)
        {
            try
            {
                string[] Files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (File.Exists(Files[0])) DataToQuery.Text = File.ReadAllText(Files[0]);
            }
            catch { Status.Content = "Failed to Load Data from Drop"; }
        }

        void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (Results.SelectedIndex == -1 && Results.HasItems) Results.SelectedIndex = 0;
            else if (Results.SelectedIndex != 0) --Results.SelectedIndex;
        }

        void Next_Click(object sender, RoutedEventArgs e)
        {
            if (Results.SelectedIndex == -1 && Results.HasItems) Results.SelectedIndex = 0;
            else if (Results.SelectedIndex != Results.Items.Count - 1) ++Results.SelectedIndex;
        }
    }
}
