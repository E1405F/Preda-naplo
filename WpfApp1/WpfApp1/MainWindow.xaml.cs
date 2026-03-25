// MainWindow.xaml.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WpfApp1.Models;
using System.Windows.Media.Effects;
using Microsoft.Win32;

namespace PredaNaplo
{
    public partial class MainWindow : Window
    {
        private List<Felhasznalo> felhasznalok = new();
        private Felhasznalo aktualisFelhasznalo;
        private JegyManager jegyManager;
        private KozlemenyManager kozlemenyManager;
        private HianyzasManager hianyzasManager;
        private Dictionary<string, string> szuloGyerekKapcsolat = new();

        private const string TITKOS_KOD = "Bturbo";
        private const string FELHASZNALOK_FAJL = "felhasznalok.txt";
        private const string JEGYEK_FAJL = "jegyek.txt";
        private const string KOZLEMENYEK_FAJL = "kozlemenyek.txt";
        private const string HIÁNYZASOK_FAJL = "hianyzasok.txt";
        private const string SZULO_GYEREK_FAJL = "szulogyerek.txt";

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                InitializeFiles();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba adatok betöltésekor: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                felhasznalok ??= new List<Felhasznalo>();
                szuloGyerekKapcsolat ??= new Dictionary<string, string>();
            }
        }

        private void InitializeFiles()
        {
            if (!File.Exists(FELHASZNALOK_FAJL)) File.Create(FELHASZNALOK_FAJL).Close();
            if (!File.Exists(JEGYEK_FAJL)) File.Create(JEGYEK_FAJL).Close();
            if (!File.Exists(KOZLEMENYEK_FAJL)) File.Create(KOZLEMENYEK_FAJL).Close();
            if (!File.Exists(HIÁNYZASOK_FAJL)) File.Create(HIÁNYZASOK_FAJL).Close();
            if (!File.Exists(SZULO_GYEREK_FAJL)) File.Create(SZULO_GYEREK_FAJL).Close();
        }

        private void LoadData()
        {
            felhasznalok = Felhasznalo.BetoltFajlbol(FELHASZNALOK_FAJL) ?? new List<Felhasznalo>();
            szuloGyerekKapcsolat = BetoltSzuloGyerekKapcsolatot() ?? new Dictionary<string, string>();

            jegyManager = new JegyManager(JEGYEK_FAJL, felhasznalok, szuloGyerekKapcsolat);
            kozlemenyManager = new KozlemenyManager(KOZLEMENYEK_FAJL, felhasznalok);
            hianyzasManager = new HianyzasManager(HIÁNYZASOK_FAJL, felhasznalok, szuloGyerekKapcsolat);
        }

        private Dictionary<string, string> BetoltSzuloGyerekKapcsolatot()
        {
            var kapcsolat = new Dictionary<string, string>();
            if (!File.Exists(SZULO_GYEREK_FAJL)) return kapcsolat;

            foreach (var sor in File.ReadAllLines(SZULO_GYEREK_FAJL))
            {
                if (string.IsNullOrWhiteSpace(sor)) continue;
                var adatok = sor.Split(';');
                if (adatok.Length >= 2)
                    kapcsolat[adatok[0]] = adatok[1];
            }
            return kapcsolat;
        }

        // Helper safe resource accessors to avoid exceptions if a resource is missing
        private Brush GetBrush(string key, Brush fallback)
        {
            return TryFindResource(key) as Brush ?? fallback;
        }

        private Style GetStyle(string key)
        {
            return TryFindResource(key) as Style;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (felhasznalok == null) LoadData();

            string username = LoginUsername?.Text ?? "";
            string password = LoginPassword?.Password ?? "";

            var user = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == username);

            if (user == null)
            {
                if (LoginMessage != null) LoginMessage.Text = "❌ Nincs ilyen felhasználó!";
                return;
            }

            if (user.Letiltva)
            {
                if (LoginMessage != null) LoginMessage.Text = "🚫 Ez a fiók le van tiltva!";
                return;
            }

            if (user.EllenorizJelszo(password))
            {
                aktualisFelhasznalo = user;
                if (LoginPanel != null) LoginPanel.Visibility = Visibility.Collapsed;
                if (MainPanel != null) MainPanel.Visibility = Visibility.Visible;
                if (LogoutButton != null) LogoutButton.Visibility = Visibility.Visible;

                if (UserNameText != null) UserNameText.Text = user.Nev;
                if (UserRoleText != null) UserRoleText.Text = user.Szerepkor;
                if (UserSchoolText != null) UserSchoolText.Text = user.IskolaNev;
                if (UserInfoText != null) UserInfoText.Text = $"{user.Nev} ({user.Szerepkor})";

                SetupMenuForRole(user.Szerepkor);
                LoadDefaultContent();
            }
            else
            {
                if (LoginMessage != null) LoginMessage.Text = "❌ Hibás jelszó!";
            }
        }

        private void SetupMenuForRole(string szerepkor)
        {
            if (MenuListBox == null) return;
            foreach (var obj in MenuListBox.Items)
            {
                if (obj is not ListBoxItem item) continue;
                switch (item.Tag?.ToString())
                {
                    case "GradeManagement":
                        item.Visibility = (szerepkor == "Tanár") ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "UserManagement":
                        item.Visibility = (szerepkor == "Adminisztrátor") ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case "Statistics":
                        item.Visibility = (szerepkor == "Igazgató" || szerepkor == "Adminisztrátor") ?
                            Visibility.Visible : Visibility.Collapsed;
                        break;
                }
            }
        }

        private void LoadDefaultContent()
        {
            ContentPanel?.Children.Clear();

            var welcomeCard = CreateCard();
            welcomeCard.Margin = new Thickness(0, 0, 0, 20);

            var welcomeStack = new StackPanel();
            welcomeStack.Children.Add(new TextBlock
            {
                Text = $"Üdvözöljük, {aktualisFelhasznalo?.Nev ?? "Felhasználó"}!",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = GetBrush("PrimaryColor", Brushes.Black),
                Margin = new Thickness(0, 0, 0, 10)
            });

            welcomeStack.Children.Add(new TextBlock
            {
                Text = $"Szerepkör: {aktualisFelhasznalo?.Szerepkor ?? "-"}",
                FontSize = 14,
                Foreground = GetBrush("TextColor", Brushes.Gray),
                Margin = new Thickness(0, 5, 0, 5)
            });

            welcomeStack.Children.Add(new TextBlock
            {
                Text = $"Iskola: {aktualisFelhasznalo?.IskolaNev ?? "-"}",
                FontSize = 14,
                Foreground = GetBrush("TextColor", Brushes.Gray),
                Margin = new Thickness(0, 5, 0, 5)
            });

            if (!string.IsNullOrEmpty(aktualisFelhasznalo?.Osztaly))
            {
                welcomeStack.Children.Add(new TextBlock
                {
                    Text = $"Osztály: {aktualisFelhasznalo.Osztaly}",
                    FontSize = 14,
                    Foreground = GetBrush("TextColor", Brushes.Gray),
                    Margin = new Thickness(0, 5, 0, 5)
                });
            }

            welcomeCard.Child = welcomeStack;
            ContentPanel?.Children.Add(welcomeCard);
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MenuListBox?.SelectedItem is ListBoxItem selected && selected.Tag != null)
            {
                switch (selected.Tag.ToString())
                {
                    case "Grades":
                        ShowGrades();
                        break;
                    case "GradeManagement":
                        ShowGradeManagement();
                        break;
                    case "Statistics":
                        ShowStatistics();
                        break;
                    case "UserManagement":
                        ShowUserManagement();
                        break;
                    case "Announcements":
                        ShowAnnouncements();
                        break;
                    case "Absences":
                        ShowAbsences();
                        break;
                }
            }
        }

        private void ShowGrades()
        {
            ContentPanel?.Children.Clear();

            var title = CreateTitle("Jegyek megtekintése");
            ContentPanel?.Children.Add(title);

            var card = CreateCard();

            var dataGrid = new DataGrid
            {
                Style = GetStyle("ModernDataGrid"),
                AutoGenerateColumns = false,
                Margin = new Thickness(0, 10, 0, 0),
                CanUserAddRows = false,
                CanUserDeleteRows = false
            };

            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Diák", Binding = new Binding("DiakNev"), Width = 150 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Tantárgy", Binding = new Binding("Tantargy"), Width = 100 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Jegy", Binding = new Binding("Ertek"), Width = 60 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Dátum", Binding = new Binding("DatumString"), Width = 100 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Megjegyzés", Binding = new Binding("Megjegyzes"), Width = 200 });

            var jegyekLista = new List<JegyMegjelenites>();

            List<Jegy> jegyek = new List<Jegy>();
            if (aktualisFelhasznalo?.Szerepkor == "Tanár" || aktualisFelhasznalo?.Szerepkor == "Igazgató" || aktualisFelhasznalo?.Szerepkor == "Adminisztrátor")
            {
                jegyek = jegyManager?.GetOsszesJegy() ?? new List<Jegy>();
            }
            else if (aktualisFelhasznalo?.Szerepkor == "Szülő")
            {
                var gyerek = GetGyermek(aktualisFelhasznalo);
                if (gyerek != null)
                    jegyek = jegyManager?.GetJegyekByDiak(gyerek.Felhasznalonev) ?? new List<Jegy>();
            }
            else if (aktualisFelhasznalo != null)
            {
                jegyek = jegyManager?.GetJegyekByDiak(aktualisFelhasznalo.Felhasznalonev) ?? new List<Jegy>();
            }

            foreach (var jegy in jegyek)
            {
                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == jegy.DiakFelhasznalonev);
                jegyekLista.Add(new JegyMegjelenites
                {
                    DiakNev = diak?.Nev ?? jegy.DiakFelhasznalonev,
                    Tantargy = jegy.Tantargy,
                    Ertek = jegy.Ertek,
                    DatumString = jegy.Datum.ToString("yyyy.MM.dd"),
                    Megjegyzes = jegy.Megjegyzes
                });
            }

            dataGrid.ItemsSource = jegyekLista;
            card.Child = dataGrid;
            ContentPanel?.Children.Add(card);

            // Show average if student
            if (aktualisFelhasznalo?.Szerepkor == "Diák" && jegyek.Any())
            {
                var avgCard = CreateCard();
                var avg = jegyek.Average(j => j.Ertek * j.Suly);
                avgCard.Child = new TextBlock
                {
                    Text = $"Súlyozott átlag: {avg:F2}",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = GetBrush("SuccessColor", Brushes.Green),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                ContentPanel?.Children.Add(avgCard);
            }
        }

        private Felhasznalo GetGyermek(Felhasznalo szulo)
        {
            if (szulo == null || szuloGyerekKapcsolat == null) return null;
            if (szuloGyerekKapcsolat.TryGetValue(szulo.Felhasznalonev, out string gyerekFelh))
            {
                return felhasznalok.FirstOrDefault(f => f.Felhasznalonev == gyerekFelh);
            }
            return null;
        }

        private void ShowGradeManagement()
        {
            ContentPanel?.Children.Clear();

            var title = CreateTitle("Jegykezelés");
            ContentPanel?.Children.Add(title);

            var newGradeCard = CreateCard();
            var newGradeBtn = new Button
            {
                Content = "Új jegy rögzítése",
                Style = GetStyle("ModernButton"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 200,
                Margin = new Thickness(0, 10, 0, 10)
            };
            newGradeBtn.Click += (s, e) => ShowNewGradeDialog();
            newGradeCard.Child = newGradeBtn;
            ContentPanel?.Children.Add(newGradeCard);

            var massGradeCard = CreateCard();
            var massGradeBtn = new Button
            {
                Content = "Jegy írás osztálynak",
                Style = GetStyle("ModernButton"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 200,
                Margin = new Thickness(0, 10, 0, 10)
            };
            massGradeBtn.Click += (s, e) => ShowMassGradeDialog();
            massGradeCard.Child = massGradeBtn;
            ContentPanel?.Children.Add(massGradeCard);

            var weightedCard = CreateCard();
            var weightedBtn = new Button
            {
                Content = "Súlyozott jegy rögzítése",
                Style = GetStyle("ModernButton"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 200,
                Margin = new Thickness(0, 10, 0, 10)
            };
            weightedBtn.Click += (s, e) => ShowWeightedGradeDialog();
            weightedCard.Child = weightedBtn;
            ContentPanel?.Children.Add(weightedCard);

            var appealsCard = CreateCard();
            var appealsBtn = new Button
            {
                Content = "Fellebbezések kezelése",
                Style = GetStyle("ModernButton"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 200,
                Margin = new Thickness(0, 10, 0, 10)
            };
            appealsBtn.Click += (s, e) => ShowAppeals();
            appealsCard.Child = appealsBtn;
            ContentPanel?.Children.Add(appealsCard);
        }

        private void ShowNewGradeDialog()
        {
            var dialog = new Window
            {
                Title = "Új jegy rögzítése",
                Width = 400,
                Height = 450,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.CanResize
            };

            var stack = new StackPanel { Margin = new Thickness(20) };

            stack.Children.Add(new TextBlock { Text = "Diák felhasználóneve:", Margin = new Thickness(0, 10, 0, 5) });
            var diakBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            stack.Children.Add(diakBox);

            stack.Children.Add(new TextBlock { Text = "Tantárgy:", Margin = new Thickness(0, 10, 0, 5) });
            var tantargyBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            stack.Children.Add(tantargyBox);

            stack.Children.Add(new TextBlock { Text = "Jegy (1-5):", Margin = new Thickness(0, 10, 0, 5) });
            var jegyBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            stack.Children.Add(jegyBox);

            stack.Children.Add(new TextBlock { Text = "Megjegyzés:", Margin = new Thickness(0, 10, 0, 5) });
            var megjegyzesBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            stack.Children.Add(megjegyzesBox);

            var message = new TextBlock { Foreground = (Brush)FindResource("DangerColor"), Margin = new Thickness(0, 10, 0, 0), TextWrapping = TextWrapping.Wrap };
            stack.Children.Add(message);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };
            var saveBtn = new Button { Content = "Mentés", Style = (Style)FindResource("ModernButton"), Width = 100 };
            var cancelBtn = new Button { Content = "Mégse", Style = (Style)FindResource("DangerButton"), Width = 100, Margin = new Thickness(10, 0, 0, 0) };
            buttonPanel.Children.Add(saveBtn);
            buttonPanel.Children.Add(cancelBtn);
            stack.Children.Add(buttonPanel);

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = stack
            };

            dialog.Content = scrollViewer;

            saveBtn.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(diakBox.Text) || string.IsNullOrWhiteSpace(tantargyBox.Text))
                {
                    message.Text = "❌ A diák és tantárgy megadása kötelező!";
                    return;
                }

                if (!int.TryParse(jegyBox.Text, out int ertek) || ertek < 1 || ertek > 5)
                {
                    message.Text = "❌ Érvénytelen jegy! (1-5)";
                    return;
                }

                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == diakBox.Text && f.Szerepkor == "Diák");
                if (diak == null)
                {
                    message.Text = "❌ Nem található ilyen diák!";
                    return;
                }

                jegyManager.UjJegy(aktualisFelhasznalo, diakBox.Text, tantargyBox.Text, ertek, megjegyzesBox.Text);
                dialog.Close();
                ShowGrades();
            };

            cancelBtn.Click += (s, e) => dialog.Close();

            dialog.ShowDialog();
        }

        private void ShowMassGradeDialog()
        {
            var dialog = new Window
            {
                Title = "Jegy írás osztálynak",
                Width = 500,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var stack = new StackPanel { Margin = new Thickness(20) };

            stack.Children.Add(new TextBlock { Text = "Osztály (pl. 10.A):", Margin = new Thickness(0, 10, 0, 5) });
            var osztalyBox = new TextBox { Style = GetStyle("ModernTextBox") };
            stack.Children.Add(osztalyBox);

            stack.Children.Add(new TextBlock { Text = "Tantárgy:", Margin = new Thickness(0, 10, 0, 5) });
            var tantargyBox = new TextBox { Style = GetStyle("ModernTextBox") };
            stack.Children.Add(tantargyBox);

            var studentsList = new ListBox { Style = GetStyle("ModernListBox"), Height = 200, Margin = new Thickness(0, 10, 0, 0) };
            stack.Children.Add(studentsList);

            var loadBtn = new Button { Content = "Diákok betöltése", Style = GetStyle("ModernButton"), Margin = new Thickness(0, 10, 0, 0) };
            loadBtn.Click += (s, e) =>
            {
                var diakok = felhasznalok.Where(f => f.Szerepkor == "Diák" && f.Osztaly == osztalyBox.Text).ToList();
                studentsList.Items.Clear();
                foreach (var diak in diakok)
                {
                    studentsList.Items.Add(new { Diak = diak, Jegy = 0 });
                }
                studentsList.DisplayMemberPath = "Diak.Nev";
            };
            stack.Children.Add(loadBtn);

            var saveBtn = new Button { Content = "Jegyek mentése", Style = GetStyle("SuccessButton"), Margin = new Thickness(0, 20, 0, 0) };
            saveBtn.Click += (s, e) =>
            {
                // Simplified: open small dialog or instruct user to use single record for now
                MessageBox.Show("Tömeges jegyírás egyszerűsített: használd az egyedi jegy rögzítését vagy bővítsd az UI-t.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            };
            stack.Children.Add(saveBtn);

            dialog.Content = stack;
            dialog.ShowDialog();
        }

        private void ShowWeightedGradeDialog()
        {
            var dialog = new Window
            {
                Title = "Súlyozott jegy rögzítése",
                Width = 400,
                Height = 520,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.CanResize
            };

            var stack = new StackPanel { Margin = new Thickness(20) };

            stack.Children.Add(new TextBlock { Text = "Diák felhasználóneve:", Margin = new Thickness(0, 10, 0, 5) });
            var diakBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            stack.Children.Add(diakBox);

            stack.Children.Add(new TextBlock { Text = "Tantárgy:", Margin = new Thickness(0, 10, 0, 5) });
            var tantargyBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            stack.Children.Add(tantargyBox);

            stack.Children.Add(new TextBlock { Text = "Jegy (1-5):", Margin = new Thickness(0, 10, 0, 5) });
            var jegyBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            stack.Children.Add(jegyBox);

            stack.Children.Add(new TextBlock { Text = "Súlyozás:", Margin = new Thickness(0, 10, 0, 5) });
            var sulyCombo = new ComboBox { Margin = new Thickness(0, 0, 0, 10) };
            sulyCombo.Items.Add(new { Name = "Normál (100%)", Value = 1.0 });
            sulyCombo.Items.Add(new { Name = "Fontos (150%)", Value = 1.5 });
            sulyCombo.Items.Add(new { Name = "Nagyon fontos (200%)", Value = 2.0 });
            sulyCombo.Items.Add(new { Name = "Különösen fontos (300%)", Value = 3.0 });
            sulyCombo.Items.Add(new { Name = "Extra fontos (400%)", Value = 4.0 });
            sulyCombo.SelectedIndex = 0;
            sulyCombo.DisplayMemberPath = "Name";
            stack.Children.Add(sulyCombo);

            stack.Children.Add(new TextBlock { Text = "Megjegyzés:", Margin = new Thickness(0, 10, 0, 5) });
            var megjegyzesBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            stack.Children.Add(megjegyzesBox);

            var message = new TextBlock { Foreground = (Brush)FindResource("DangerColor"), Margin = new Thickness(0, 10, 0, 0), TextWrapping = TextWrapping.Wrap };
            stack.Children.Add(message);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };
            var saveBtn = new Button { Content = "Mentés", Style = (Style)FindResource("ModernButton"), Width = 100 };
            var cancelBtn = new Button { Content = "Mégse", Style = (Style)FindResource("DangerButton"), Width = 100, Margin = new Thickness(10, 0, 0, 0) };
            buttonPanel.Children.Add(saveBtn);
            buttonPanel.Children.Add(cancelBtn);
            stack.Children.Add(buttonPanel);

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = stack
            };

            dialog.Content = scrollViewer;

            saveBtn.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(diakBox.Text) || string.IsNullOrWhiteSpace(tantargyBox.Text))
                {
                    message.Text = "❌ A diák és tantárgy megadása kötelező!";
                    return;
                }

                if (!int.TryParse(jegyBox.Text, out int ertek) || ertek < 1 || ertek > 5)
                {
                    message.Text = "❌ Érvénytelen jegy! (1-5)";
                    return;
                }

                dynamic selected = sulyCombo.SelectedItem;
                double suly = selected.Value;

                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == diakBox.Text && f.Szerepkor == "Diák");
                if (diak == null)
                {
                    message.Text = "❌ Nem található ilyen diák!";
                    return;
                }

                jegyManager.UjJegySullyal(aktualisFelhasznalo, diakBox.Text, tantargyBox.Text, ertek, suly, megjegyzesBox.Text);
                dialog.Close();
                ShowGrades();
            };

            cancelBtn.Click += (s, e) => dialog.Close();

            dialog.ShowDialog();
        }

        private void ShowAppeals()
        {
            ContentPanel?.Children.Clear();

            var title = CreateTitle("Fellebbezések kezelése");
            ContentPanel?.Children.Add(title);

            var appeals = jegyManager?.GetFellebbezesek() ?? new List<Jegy>();

            if (!appeals.Any())
            {
                var card = CreateCard();
                card.Child = new TextBlock
                {
                    Text = "Nincsenek függőben lévő fellebbezések.",
                    FontSize = 14,
                    Foreground = GetBrush("TextColor", Brushes.Gray),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                ContentPanel?.Children.Add(card);
                return;
            }

            foreach (var jegy in appeals)
            {
                var card = CreateCard();
                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == jegy.DiakFelhasznalonev);

                var stack = new StackPanel();
                stack.Children.Add(new TextBlock
                {
                    Text = $"Diák: {diak?.Nev ?? jegy.DiakFelhasznalonev}",
                    FontWeight = FontWeights.Bold,
                    FontSize = 14
                });
                stack.Children.Add(new TextBlock { Text = $"Tantárgy: {jegy.Tantargy}" });
                stack.Children.Add(new TextBlock { Text = $"Jegy: {jegy.Ertek}" });
                stack.Children.Add(new TextBlock { Text = $"Dátum: {jegy.Datum:yyyy.MM.dd}" });
                stack.Children.Add(new TextBlock { Text = $"Indoklás: {jegy.FellebbezesIndoklas}", TextWrapping = TextWrapping.Wrap });

                var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 0) };
                var acceptBtn = new Button { Content = "Elfogadás", Style = GetStyle("SuccessButton"), Width = 100 };
                var rejectBtn = new Button { Content = "Elutasítás", Style = GetStyle("DangerButton"), Width = 100, Margin = new Thickness(10, 0, 0, 0) };

                acceptBtn.Click += (s, e) =>
                {
                    var dialog = new Window
                    {
                        Title = "Jegy módosítása",
                        Width = 300,
                        Height = 200,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        Owner = this
                    };
                    var dialogStack = new StackPanel { Margin = new Thickness(20) };
                    dialogStack.Children.Add(new TextBlock { Text = "Új jegy érték (1-5):" });
                    var newGradeBox = new TextBox { Style = GetStyle("ModernTextBox") };
                    dialogStack.Children.Add(newGradeBox);
                    var saveBtn = new Button { Content = "Mentés", Style = GetStyle("ModernButton"), Margin = new Thickness(0, 20, 0, 0) };
                    saveBtn.Click += (s2, e2) =>
                    {
                        if (int.TryParse(newGradeBox.Text, out int ujErtek) && ujErtek >= 1 && ujErtek <= 5)
                        {
                            jegyManager?.FellebbezesElfogadas(jegy, ujErtek);
                            dialog.Close();
                            ShowAppeals();
                        }
                    };
                    dialogStack.Children.Add(saveBtn);
                    dialog.Content = dialogStack;
                    dialog.ShowDialog();
                };

                rejectBtn.Click += (s, e) =>
                {
                    jegyManager?.FellebbezesElutasitas(jegy);
                    ShowAppeals();
                };

                buttonPanel.Children.Add(acceptBtn);
                buttonPanel.Children.Add(rejectBtn);
                stack.Children.Add(buttonPanel);
                card.Child = stack;
                ContentPanel?.Children.Add(card);
            }
        }

        private void ShowStatistics()
        {
            ContentPanel?.Children.Clear();

            var title = CreateTitle("Statisztikák");
            ContentPanel?.Children.Add(title);

            // Class averages
            var classAvgCard = CreateCard();
            var classAvgTitle = new TextBlock
            {
                Text = "Osztályátlagok",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = GetBrush("PrimaryColor", Brushes.Black),
                Margin = new Thickness(0, 0, 0, 10)
            };

            var classAvgGrid = new DataGrid
            {
                Style = GetStyle("ModernDataGrid"),
                AutoGenerateColumns = false,
                Height = 200
            };
            classAvgGrid.Columns.Add(new DataGridTextColumn { Header = "Osztály", Binding = new Binding("Osztaly"), Width = 100 });
            classAvgGrid.Columns.Add(new DataGridTextColumn { Header = "Átlag", Binding = new Binding("Atlag"), Width = 100 });

            var osztalyAtlagok = jegyManager?.GetOsztalyAtlagok() ?? new Dictionary<string, double>();
            var classAvgList = osztalyAtlagok.Select(o => new { Osztaly = o.Key, Atlag = o.Value }).ToList();
            classAvgGrid.ItemsSource = classAvgList;

            var classAvgStack = new StackPanel();
            classAvgStack.Children.Add(classAvgTitle);
            classAvgStack.Children.Add(classAvgGrid);
            classAvgCard.Child = classAvgStack;
            ContentPanel?.Children.Add(classAvgCard);

            // Absence statistics
            var absenceCard = CreateCard();
            var absenceTitle = new TextBlock
            {
                Text = "Hiányzási statisztikák",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = GetBrush("PrimaryColor", Brushes.Black),
                Margin = new Thickness(0, 0, 0, 10)
            };

            var absenceGrid = new DataGrid
            {
                Style = GetStyle("ModernDataGrid"),
                AutoGenerateColumns = false,
                Height = 200
            };
            absenceGrid.Columns.Add(new DataGridTextColumn { Header = "Osztály", Binding = new Binding("Osztaly"), Width = 100 });
            absenceGrid.Columns.Add(new DataGridTextColumn { Header = "Hiányzások száma", Binding = new Binding("Szam"), Width = 150 });

            var hianyzasStat = hianyzasManager?.GetHianyzasStatisztikak() ?? new Dictionary<string, int>();
            var absenceList = hianyzasStat.Select(s => new { Osztaly = s.Key, Szam = s.Value }).ToList();
            absenceGrid.ItemsSource = absenceList;

            var absenceStack = new StackPanel();
            absenceStack.Children.Add(absenceTitle);
            absenceStack.Children.Add(absenceGrid);
            absenceCard.Child = absenceStack;
            ContentPanel?.Children.Add(absenceCard);
        }

        private void ShowUserManagement()
        {
            ContentPanel?.Children.Clear();

            var title = CreateTitle("Felhasználók kezelése");
            ContentPanel?.Children.Add(title);

            var dataGrid = new DataGrid
            {
                Style = GetStyle("ModernDataGrid"),
                AutoGenerateColumns = false,
                Margin = new Thickness(0, 10, 0, 10),
                CanUserAddRows = false
            };

            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Név", Binding = new Binding("Nev"), Width = 150 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Felhasználónév", Binding = new Binding("Felhasznalonev"), Width = 120 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Szerepkör", Binding = new Binding("Szerepkor"), Width = 100 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Osztály", Binding = new Binding("Osztaly"), Width = 80 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Státusz", Binding = new Binding("Statusz"), Width = 80 });

            var userList = felhasznalok.Select(f => new
            {
                f.Nev,
                f.Felhasznalonev,
                f.Szerepkor,
                f.Osztaly,
                Statusz = f.Letiltva ? "Letiltva" : "Aktív"
            }).ToList();

            dataGrid.ItemsSource = userList;

            var card = CreateCard();
            card.Child = dataGrid;
            ContentPanel?.Children.Add(card);

            // Action buttons
            var actionPanel = new WrapPanel { Margin = new Thickness(0, 10, 0, 0) };

            var toggleBtn = new Button { Content = "Letiltás/Feloldás", Style = GetStyle("ModernButton"), Width = 120, Margin = new Thickness(5) };
            var changePasswordBtn = new Button { Content = "Jelszó változtatás", Style = GetStyle("ModernButton"), Width = 120, Margin = new Thickness(5) };
            var deleteBtn = new Button { Content = "Fiók törlése", Style = GetStyle("DangerButton"), Width = 120, Margin = new Thickness(5) };

            actionPanel.Children.Add(toggleBtn);
            actionPanel.Children.Add(changePasswordBtn);
            actionPanel.Children.Add(deleteBtn);

            ContentPanel?.Children.Add(actionPanel);

            // Add register button
            var registerCard = CreateCard();
            var registerBtn = new Button
            {
                Content = "Új felhasználó regisztrálása",
                Style = GetStyle("SuccessButton"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 200
            };
            registerBtn.Click += (s, e) => ShowRegistrationDialog();
            registerCard.Child = registerBtn;
            ContentPanel?.Children.Add(registerCard);
        }

        private void ShowRegistrationDialog()
        {
            var dialog = new Window
            {
                Title = "Új felhasználó regisztrálása",
                Width = 400,
                Height = 620,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var mainStack = new StackPanel { Margin = new Thickness(20) };

            mainStack.Children.Add(new TextBlock { Text = "Felhasználónév:", Margin = new Thickness(0, 10, 0, 5) });
            var usernameBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            mainStack.Children.Add(usernameBox);

            mainStack.Children.Add(new TextBlock { Text = "Jelszó:", Margin = new Thickness(0, 10, 0, 5) });
            var passwordBox = new PasswordBox { Style = (Style)FindResource("ModernPasswordBox") };
            mainStack.Children.Add(passwordBox);

            mainStack.Children.Add(new TextBlock { Text = "Teljes név:", Margin = new Thickness(0, 10, 0, 5) });
            var nameBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            mainStack.Children.Add(nameBox);

            mainStack.Children.Add(new TextBlock { Text = "Iskola neve:", Margin = new Thickness(0, 10, 0, 5) });
            var schoolBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            mainStack.Children.Add(schoolBox);

            mainStack.Children.Add(new TextBlock { Text = "Szerepkör:", Margin = new Thickness(0, 10, 0, 5) });
            var roleCombo = new ComboBox { Margin = new Thickness(0, 0, 0, 10) };
            roleCombo.Items.Add("Diák");
            roleCombo.Items.Add("Tanár");
            roleCombo.Items.Add("Szülő");
            roleCombo.Items.Add("Igazgató");
            roleCombo.Items.Add("Adminisztrátor");
            roleCombo.SelectedIndex = 0;
            mainStack.Children.Add(roleCombo);

            mainStack.Children.Add(new TextBlock { Text = "Osztály (Diák/Tanár esetén):", Margin = new Thickness(0, 10, 0, 5) });
            var classBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            mainStack.Children.Add(classBox);

            mainStack.Children.Add(new TextBlock { Text = "Titkos kód (Igazgató/Adminisztrátor/Tanár):", Margin = new Thickness(0, 10, 0, 5) });
            var codeBox = new PasswordBox { Style = (Style)FindResource("ModernPasswordBox") };
            mainStack.Children.Add(codeBox);

            var message = new TextBlock { Foreground = (Brush)FindResource("DangerColor"), Margin = new Thickness(0, 10, 0, 0), TextWrapping = TextWrapping.Wrap };
            mainStack.Children.Add(message);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };
            var saveBtn = new Button { Content = "Regisztrálás", Style = (Style)FindResource("ModernButton"), Width = 100 };
            var cancelBtn = new Button { Content = "Mégse", Style = (Style)FindResource("DangerButton"), Width = 100, Margin = new Thickness(10, 0, 0, 0) };
            buttonPanel.Children.Add(saveBtn);
            buttonPanel.Children.Add(cancelBtn);
            mainStack.Children.Add(buttonPanel);

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = mainStack
            };

            dialog.Content = scrollViewer;

            // Eseménykezelők
            saveBtn.Click += (s, e) =>
            {
                string role = roleCombo.SelectedItem.ToString();

                if ((role == "Igazgató" || role == "Adminisztrátor" || role == "Tanár") && codeBox.Password != TITKOS_KOD)
                {
                    message.Text = "❌ Hibás titkos kód!";
                    return;
                }

                var reg = new Regisztracio(felhasznalok, FELHASZNALOK_FAJL, szuloGyerekKapcsolat, SZULO_GYEREK_FAJL, TITKOS_KOD);
                bool success = reg.UjFelhasznalo(
                    usernameBox.Text,
                    passwordBox.Password,
                    nameBox.Text,
                    schoolBox.Text,
                    role,
                    classBox.Text,
                    null);

                if (success)
                {
                    LoadData();
                    message.Text = "✅ Regisztráció sikeres!";
                    message.Foreground = (Brush)FindResource("SuccessColor");
                    dialog.Close();
                    ShowUserManagement();
                }
                else
                {
                    message.Text = "❌ A felhasználónév már létezik!";
                }
            };

            cancelBtn.Click += (s, e) => dialog.Close();

            dialog.ShowDialog();
        }

        private void ShowAnnouncements()
        {
            ContentPanel?.Children.Clear();

            var title = CreateTitle("Közlemények");
            ContentPanel?.Children.Add(title);

            // New announcement button for teachers/admins
            if (aktualisFelhasznalo?.Szerepkor == "Tanár" || aktualisFelhasznalo?.Szerepkor == "Igazgató" || aktualisFelhasznalo?.Szerepkor == "Adminisztrátor")
            {
                var newAnnouncementCard = CreateCard();
                var newBtn = new Button
                {
                    Content = "Új közlemény írása",
                    Style = GetStyle("ModernButton"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 200
                };
                newBtn.Click += (s, e) => ShowNewAnnouncementDialog();
                newAnnouncementCard.Child = newBtn;
                ContentPanel?.Children.Add(newAnnouncementCard);
            }

            // Display announcements
            var announcements = kozlemenyManager?.GetKozlemenyekByUser(aktualisFelhasznalo?.Felhasznalonev ?? "") ?? new List<Kozlemeny>();

            if (!announcements.Any())
            {
                var card = CreateCard();
                card.Child = new TextBlock
                {
                    Text = "Nincsenek közlemények.",
                    FontSize = 14,
                    Foreground = GetBrush("TextColor", Brushes.Gray),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                ContentPanel?.Children.Add(card);
                return;
            }

            foreach (var koz in announcements.OrderByDescending(k => k.Datum))
            {
                var card = CreateCard();
                var stack = new StackPanel();

                stack.Children.Add(new TextBlock
                {
                    Text = koz.Cim,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = GetBrush("PrimaryColor", Brushes.Black)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = koz.Tartalom,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 10)
                });

                stack.Children.Add(new TextBlock
                {
                    Text = $"Feladó: {koz.FeladoFelhasznalonev} - {koz.Datum:yyyy.MM.dd HH:mm}",
                    FontSize = 11,
                    Foreground = GetBrush("WarningColor", Brushes.Orange)
                });

                card.Child = stack;
                ContentPanel?.Children.Add(card);
            }
        }

        private void ShowNewAnnouncementDialog()
        {
            var dialog = new Window
            {
                Title = "Új közlemény",
                Width = 450,
                Height = 450,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.CanResize
            };

            var stack = new StackPanel { Margin = new Thickness(20) };

            stack.Children.Add(new TextBlock { Text = "Cím:", Margin = new Thickness(0, 10, 0, 5) });
            var titleBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            stack.Children.Add(titleBox);

            stack.Children.Add(new TextBlock { Text = "Tartalom:", Margin = new Thickness(0, 10, 0, 5) });
            var contentBox = new TextBox { Style = (Style)FindResource("ModernTextBox"), Height = 150, TextWrapping = TextWrapping.Wrap };
            stack.Children.Add(contentBox);

            var message = new TextBlock { Foreground = (Brush)FindResource("DangerColor"), Margin = new Thickness(0, 10, 0, 0) };
            stack.Children.Add(message);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };
            var sendBtn = new Button { Content = "Küldés", Style = (Style)FindResource("ModernButton"), Width = 100 };
            var cancelBtn = new Button { Content = "Mégse", Style = (Style)FindResource("DangerButton"), Width = 100, Margin = new Thickness(10, 0, 0, 0) };
            buttonPanel.Children.Add(sendBtn);
            buttonPanel.Children.Add(cancelBtn);
            stack.Children.Add(buttonPanel);

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = stack
            };

            dialog.Content = scrollViewer;

            sendBtn.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(titleBox.Text) || string.IsNullOrWhiteSpace(contentBox.Text))
                {
                    message.Text = "❌ Cím és tartalom megadása kötelező!";
                    return;
                }

                kozlemenyManager.UjKozlemeny(aktualisFelhasznalo, titleBox.Text, contentBox.Text);
                message.Text = "✅ Közlemény elküldve!";
                message.Foreground = (Brush)FindResource("SuccessColor");
                dialog.Close();
                ShowAnnouncements();
            };

            cancelBtn.Click += (s, e) => dialog.Close();

            dialog.ShowDialog();
        }

        private void ShowAbsences()
        {
            ContentPanel?.Children.Clear();

            var title = CreateTitle("Hiányzások");
            ContentPanel?.Children.Add(title);

            // Add absence recording for teachers
            if (aktualisFelhasznalo?.Szerepkor == "Tanár")
            {
                var recordCard = CreateCard();
                var recordBtn = new Button
                {
                    Content = "Hiányzás rögzítése",
                    Style = GetStyle("ModernButton"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 150
                };
                recordBtn.Click += (s, e) => ShowRecordAbsenceDialog();
                recordCard.Child = recordBtn;
                ContentPanel?.Children.Add(recordCard);
            }

            // Add absence justification for parents/teachers
            if (aktualisFelhasznalo?.Szerepkor == "Szülő" || aktualisFelhasznalo?.Szerepkor == "Tanár" || aktualisFelhasznalo?.Szerepkor == "Igazgató")
            {
                var justifyCard = CreateCard();
                var justifyBtn = new Button
                {
                    Content = "Hiányzás igazolása",
                    Style = GetStyle("SuccessButton"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 150,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                justifyBtn.Click += (s, e) => ShowJustifyAbsenceDialog();
                justifyCard.Child = justifyBtn;
                ContentPanel?.Children.Add(justifyCard);
            }

            // Display absences
            var dataGrid = new DataGrid
            {
                Style = GetStyle("ModernDataGrid"),
                AutoGenerateColumns = false,
                Margin = new Thickness(0, 10, 0, 0),
                CanUserAddRows = false
            };

            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Diák", Binding = new Binding("DiakNev"), Width = 150 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Dátum", Binding = new Binding("Datum"), Width = 100 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Óra", Binding = new Binding("Ora"), Width = 150 });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Státusz", Binding = new Binding("Statusz"), Width = 100 });

            List<Hianyzas> hianyzasok = new List<Hianyzas>();
            if (aktualisFelhasznalo?.Szerepkor == "Tanár" || aktualisFelhasznalo?.Szerepkor == "Igazgató" || aktualisFelhasznalo?.Szerepkor == "Adminisztrátor")
            {
                hianyzasok = hianyzasManager?.GetOsszesHianyzas() ?? new List<Hianyzas>();
            }
            else if (aktualisFelhasznalo?.Szerepkor == "Szülő")
            {
                var gyerek = GetGyermek(aktualisFelhasznalo);
                if (gyerek != null)
                    hianyzasok = hianyzasManager?.GetHianyzasokByDiak(gyerek.Felhasznalonev) ?? new List<Hianyzas>();
            }
            else if (aktualisFelhasznalo != null)
            {
                hianyzasok = hianyzasManager?.GetHianyzasokByDiak(aktualisFelhasznalo.Felhasznalonev) ?? new List<Hianyzas>();
            }

            var absenceList = hianyzasok.Select(h =>
            {
                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == h.DiakFelhasznalonev);
                return new
                {
                    DiakNev = diak?.Nev ?? h.DiakFelhasznalonev,
                    Datum = h.Datum.ToString("yyyy.MM.dd"),
                    Ora = h.Ora,
                    Statusz = h.Igazolt ? "Igazolt" : "Igazolatlan"
                };
            }).ToList();

            dataGrid.ItemsSource = absenceList;

            var card = CreateCard();
            card.Child = dataGrid;
            ContentPanel?.Children.Add(card);
        }

        private void ShowRecordAbsenceDialog()
        {
            var dialog = new Window
            {
                Title = "Hiányzás rögzítése",
                Width = 400,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.CanResize
            };

            var stack = new StackPanel { Margin = new Thickness(20) };

            stack.Children.Add(new TextBlock { Text = "Diák felhasználóneve:", Margin = new Thickness(0, 10, 0, 5) });
            var diakBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            stack.Children.Add(diakBox);

            stack.Children.Add(new TextBlock { Text = "Dátum (üresen hagyva ma):", Margin = new Thickness(0, 10, 0, 5) });
            var dateBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            stack.Children.Add(dateBox);

            stack.Children.Add(new TextBlock { Text = "Óra/Tantárgy:", Margin = new Thickness(0, 10, 0, 5) });
            var lessonBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            stack.Children.Add(lessonBox);

            var message = new TextBlock { Foreground = (Brush)FindResource("DangerColor"), Margin = new Thickness(0, 10, 0, 0) };
            stack.Children.Add(message);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };
            var saveBtn = new Button { Content = "Rögzítés", Style = (Style)FindResource("ModernButton"), Width = 100 };
            var cancelBtn = new Button { Content = "Mégse", Style = (Style)FindResource("DangerButton"), Width = 100, Margin = new Thickness(10, 0, 0, 0) };
            buttonPanel.Children.Add(saveBtn);
            buttonPanel.Children.Add(cancelBtn);
            stack.Children.Add(buttonPanel);

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = stack
            };

            dialog.Content = scrollViewer;

            saveBtn.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(diakBox.Text) || string.IsNullOrWhiteSpace(lessonBox.Text))
                {
                    message.Text = "❌ Diák és óra megadása kötelező!";
                    return;
                }

                DateTime datum = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(dateBox.Text))
                {
                    if (!DateTime.TryParse(dateBox.Text, out datum))
                    {
                        message.Text = "❌ Érvénytelen dátum formátum!";
                        return;
                    }
                }

                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == diakBox.Text && f.Szerepkor == "Diák");
                if (diak == null)
                {
                    message.Text = "❌ Nem található ilyen diák!";
                    return;
                }

                hianyzasManager.HianyzasRogzitese(aktualisFelhasznalo, diakBox.Text, datum, lessonBox.Text);
                dialog.Close();
                ShowAbsences();
            };

            cancelBtn.Click += (s, e) => dialog.Close();

            dialog.ShowDialog();
        }

        private void ShowJustifyAbsenceDialog()
        {
            var dialog = new Window
            {
                Title = "Hiányzás igazolása",
                Width = 450,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var stack = new StackPanel { Margin = new Thickness(20) };

            // Get unverified absences
            List<Hianyzas> igazolatlanok = new List<Hianyzas>();

            if (aktualisFelhasznalo?.Szerepkor == "Tanár" || aktualisFelhasznalo?.Szerepkor == "Igazgató")
            {
                igazolatlanok = hianyzasManager?.GetIgazolatlanHianyzasok() ?? new List<Hianyzas>();
            }
            else if (aktualisFelhasznalo?.Szerepkor == "Szülő")
            {
                var gyerek = GetGyermek(aktualisFelhasznalo);
                if (gyerek != null)
                    igazolatlanok = hianyzasManager?.GetIgazolatlanHianyzasokByDiak(gyerek.Felhasznalonev) ?? new List<Hianyzas>();
            }

            if (!igazolatlanok.Any())
            {
                stack.Children.Add(new TextBlock
                {
                    Text = "Nincsenek igazolatlan hiányzások.",
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                });

                var closeBtn = new Button { Content = "Bezárás", Style = GetStyle("ModernButton"), Width = 100, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };
                closeBtn.Click += (s, e) => dialog.Close();
                stack.Children.Add(closeBtn);

                dialog.Content = stack;
                dialog.ShowDialog();
                return;
            }

            var listBox = new ListBox { Style = GetStyle("ModernListBox"), Height = 200, DisplayMemberPath = "DisplayText" };
            var items = new List<dynamic>();
            for (int i = 0; i < igazolatlanok.Count; i++)
            {
                var h = igazolatlanok[i];
                var diak = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == h.DiakFelhasznalonev);
                items.Add(new { Id = i, DisplayText = $"{diak?.Nev ?? h.DiakFelhasznalonev} - {h.Datum:yyyy.MM.dd} - {h.Ora}" });
            }
            listBox.ItemsSource = items;
            stack.Children.Add(listBox);

            var message = new TextBlock { Foreground = GetBrush("DangerColor", Brushes.Red), Margin = new Thickness(0, 10, 0, 0) };
            stack.Children.Add(message);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };
            var justifyBtn = new Button { Content = "Igazolás", Style = GetStyle("SuccessButton"), Width = 100 };
            var cancelBtn = new Button { Content = "Mégse", Style = GetStyle("DangerButton"), Width = 100, Margin = new Thickness(10, 0, 0, 0) };

            justifyBtn.Click += (s, e) =>
            {
                if (listBox.SelectedItem == null)
                {
                    message.Text = "❌ Válassz ki egy hiányzást!";
                    return;
                }

                dynamic selected = listBox.SelectedItem;
                var selectedAbsence = igazolatlanok[selected.Id];
                hianyzasManager?.Igazolas(selectedAbsence);
                dialog.Close();
                ShowAbsences();
            };

            cancelBtn.Click += (s, e) => dialog.Close();

            buttonPanel.Children.Add(justifyBtn);
            buttonPanel.Children.Add(cancelBtn);
            stack.Children.Add(buttonPanel);

            dialog.Content = stack;
            dialog.ShowDialog();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            aktualisFelhasznalo = null;
            if (LoginPanel != null) LoginPanel.Visibility = Visibility.Visible;
            if (MainPanel != null) MainPanel.Visibility = Visibility.Collapsed;
            if (LogoutButton != null) LogoutButton.Visibility = Visibility.Collapsed;
            if (LoginUsername != null) LoginUsername.Text = "";
            if (LoginPassword != null) LoginPassword.Password = "";
            if (LoginMessage != null) LoginMessage.Text = "";
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ShowRegistrationDialog();
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Window
            {
                Title = "Elfelejtett jelszó",
                Width = 350,
                Height = 280,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };

            var mainStack = new StackPanel { Margin = new Thickness(20) };

            mainStack.Children.Add(new TextBlock { Text = "Felhasználónév:", Margin = new Thickness(0, 10, 0, 5) });
            var usernameBox = new TextBox { Style = (Style)FindResource("ModernTextBox") };
            mainStack.Children.Add(usernameBox);

            mainStack.Children.Add(new TextBlock { Text = "Új jelszó:", Margin = new Thickness(0, 10, 0, 5) });
            var newPasswordBox = new PasswordBox { Style = (Style)FindResource("ModernPasswordBox") };
            mainStack.Children.Add(newPasswordBox);

            var message = new TextBlock { Foreground = (Brush)FindResource("DangerColor"), Margin = new Thickness(0, 10, 0, 0) };
            mainStack.Children.Add(message);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 20, 0, 0) };
            var changeBtn = new Button { Content = "Módosítás", Style = (Style)FindResource("ModernButton"), Width = 100 };
            var cancelBtn = new Button { Content = "Mégse", Style = (Style)FindResource("DangerButton"), Width = 100, Margin = new Thickness(10, 0, 0, 0) };
            buttonPanel.Children.Add(changeBtn);
            buttonPanel.Children.Add(cancelBtn);
            mainStack.Children.Add(buttonPanel);

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = mainStack
            };

            dialog.Content = scrollViewer;

            changeBtn.Click += (s2, e2) =>
            {
                var user = felhasznalok.FirstOrDefault(f => f.Felhasznalonev == usernameBox.Text);
                if (user == null)
                {
                    message.Text = "❌ Nem található ilyen felhasználó!";
                    return;
                }

                if (string.IsNullOrWhiteSpace(newPasswordBox.Password))
                {
                    message.Text = "❌ A jelszó nem lehet üres!";
                    return;
                }

                user.JelszoMegvaltoztatasa(newPasswordBox.Password);
                FrissitFelhasznalokFajlt();
                message.Text = "✅ Jelszó sikeresen megváltoztatva!";
                message.Foreground = (Brush)FindResource("SuccessColor");
            };

            cancelBtn.Click += (s2, e2) => dialog.Close();

            dialog.ShowDialog();
        }

        private void FrissitFelhasznalokFajlt()
        {
            try
            {
                var sorok = felhasznalok.Select(f => f.ToFileFormat());
                File.WriteAllLines(FELHASZNALOK_FAJL, sorok);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a fájl frissítésekor: {ex.Message}", "Hiba",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Border CreateCard()
        {
            return new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 15),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    BlurRadius = 10,
                    Opacity = 0.2,
                    ShadowDepth = 2
                }
            };
        }

        private TextBlock CreateTitle(string title)
        {
            return new TextBlock
            {
                Text = title,
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = GetBrush("PrimaryColor", Brushes.Black),
                Margin = new Thickness(0, 0, 0, 20)
            };
        }
    }

    public class JegyMegjelenites
    {
        public string DiakNev { get; set; }
        public string Tantargy { get; set; }
        public int Ertek { get; set; }
        public string DatumString { get; set; }
        public string Megjegyzes { get; set; }
    }
}