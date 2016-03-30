using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Autobusy
{
    /// <summary>
    /// Interaction logic for Rozklad.xaml
    /// </summary>
    public partial class Rozklad : Window
    {

        private void Dodaj(Grid grid, string text, int row)
        {
            var gridRow = new RowDefinition();
            gridRow.Height= GridLength.Auto;
            grid.RowDefinitions.Add(gridRow);
            var label = new Label
            {
                Content = text,
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 15
            };
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, row);
            grid.Children.Add(label);
            var border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 0, 1)
            };
            Grid.SetRow(border,row);
            Grid.SetColumnSpan(border,2);
            grid.Children.Add(border);
        }


        private void Rozdziel(Funkcje.LinkItem item, Grid grid, int row,int dlugosc)
        {
            int minuty;
            var tekst = item.Text.Substring(dlugosc,item.Text.Length-dlugosc);
            var label = new Label();
            Grid.SetColumn(label, 1);
            Grid.SetRow(label, row);
            label.Foreground = new SolidColorBrush(Colors.Black);
            label.FontSize = 15;
            grid.Children.Add(label);
            for (var i = 0; i < tekst.Length; i++)
            {
                
                if (i + 3 <= tekst.Length)
                {
                    var test = Int32.TryParse(tekst.Substring(i,3), out minuty);
                    if (test)
                    {
                        label.Content = label.Content + tekst.Substring(i,2) + " ";
                        i++;

                    }
                    else
                    {
                        label.Content = label.Content + tekst.Substring(i,3) + " ";
                        i = i + 2;
                    }
                }
                else
                {
                    label.Content = label.Content + tekst.Substring(i,2) + " ";
                    i = tekst.Length;
                }
            }
        }
        public Rozklad(string html)
        {
            InitializeComponent();
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            var strona = client.DownloadString(html);
            strona = strona.Substring(strona.IndexOf("id=\"rozklad_1\""),
                strona.LastIndexOf("</table>") - strona.IndexOf("id=\"rozklad_1\""));
            var lista = new List<Funkcje.LinkItem>();
            var lista2 = new List<Funkcje.LinkItem>();
            var dzien = new string[3];
            lista = Funkcje.Findkierunek(strona);
            lista2 = Funkcje.Findtbody(strona);
            dzien[0] = lista[0].Text;
            var i = 0;
            var j = 1;
            var licznikgrid = 0;
            var row = 0;
            var grid = PierwszyGrid;
            foreach (var godzina in lista2)
            {
                int aktualny;
                if (i < lista2.Count - 1)
                {
                    Int32.TryParse(lista2[i].Text, out aktualny);
                    int nastepny;
                    var nast = Int32.TryParse(lista2[i + 1].Text, out nastepny);
                    if (nast)
                    {
                        if (aktualny < nastepny)
                        {
                            Dodaj(grid, godzina.Text, row);
                            Rozdziel(lista[j],grid,row,lista2[i].Text.Length);
                            i++;
                            j++;
                            row++;
                        }
                        else
                        {
                            row++;
                            Dodaj(grid, godzina.Text, row);
                            Rozdziel(lista[j], grid, row, lista2[i].Text.Length);
                            row = 0;
                            licznikgrid++;
                            i++;
                            j=j+2;
                            if (licznikgrid == 1)
                            {
                                grid = DrugiGrid;
                                dzien[1] = lista[j-1].Text;
                            }
                            else
                            {
                                grid = TrzeciGrid;
                                dzien[2] = lista[j - 1].Text;
                            }
                        }
                    }
                    else
                    {
                        Dodaj(grid, godzina.Text, row);
                        Rozdziel(lista[j], grid, row, lista2[i].Text.Length);
                        i++;
                        j = j + 2;
                        row++;
                        licznikgrid++;
                        if (licznikgrid == 1)
                        {
                            grid = DrugiGrid;
                        }
                        else
                        {
                            grid = TrzeciGrid;
                        }
                    }
                }
                else
                {
                    var akt = Int32.TryParse(lista2[i].Text, out aktualny);
                    if (akt)
                    {
                        Dodaj(grid, godzina.Text, row);
                        Rozdziel(lista[j], grid, row, lista2[i].Text.Length);

                    }
                    else
                    {
                        Dodaj(grid, "---", row);

                    }
                }
            }

            PierwszyLabel.Content = dzien[0];
            DrugiLabel.Content = dzien[1];
            TrzeciLabel.Content = dzien[2];
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}

