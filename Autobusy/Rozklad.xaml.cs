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
            RowDefinition gridRow = new RowDefinition();
            grid.RowDefinitions.Add(gridRow);
            Label label = new Label();
            label.Content = text;
            label.Foreground = new SolidColorBrush(Colors.Black);
            label.FontSize = 15;
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, row);
            grid.Children.Add(label);
        }


        private void Rozdziel(Funkcje.LinkItem item, Grid grid, int row,int dlugosc)
        {
            int minuty;
            string tekst = item.Text.Substring(dlugosc,item.Text.Length-dlugosc);
            Label label = new Label();
            Grid.SetColumn(label, 1);
            Grid.SetRow(label, row);
            label.Foreground = new SolidColorBrush(Colors.Black);
            label.FontSize = 15;
            grid.Children.Add(label);
            for (int i = 0; i < tekst.Length; i++)
            {
                
                if (i + 3 <= tekst.Length)
                {
                    bool test = Int32.TryParse(tekst.Substring(i,3), out minuty);
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
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string strona = client.DownloadString(html);
            strona = strona.Substring(strona.IndexOf("id=\"rozklad_1\""),
                strona.LastIndexOf("</table>") - strona.IndexOf("id=\"rozklad_1\""));
            List<Funkcje.LinkItem> lista = new List<Funkcje.LinkItem>();
            List<Funkcje.LinkItem> lista2 = new List<Funkcje.LinkItem>();
            lista = Funkcje.Findkierunek(strona);
            lista2 = Funkcje.Findtbody(strona);
            int i = 0;
            int j = 1;
            int licznikgrid = 0;
            int row = 0;
            int aktualny, nastepny;
            bool nast;
            Grid grid = PierwszyGrid;
            foreach (var godzina in lista2)
            {

                if (i < lista2.Count - 1)
                {
                    Int32.TryParse(lista2[i].Text, out aktualny);
                    nast = Int32.TryParse(lista2[i + 1].Text, out nastepny);
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
                            }
                            else
                            {
                                grid = TrzeciGrid;
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
                    bool akt = Int32.TryParse(lista2[i].Text, out aktualny);
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
        }
    }
}

