using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Autobusy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<string> listaprzystankow;
        private List<string> nrprzystanku; 
        private string htmlkierunek;
        

        public MainWindow()
        {
            InitializeComponent();
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string html = client.DownloadString("http://www.komunikacja.bialystok.pl/?page=rozklad_jazdy");

            string linie = html.Substring(html.IndexOf("<!-- START box: tabela_linii -->"),
                html.LastIndexOf("<!-- END box: tabela_linii -->")- html.IndexOf("<!-- START box: tabela_linii -->"));

            List<Funkcje.LinkItem> numbers = new List<Funkcje.LinkItem>();
            numbers = Funkcje.Findlink(linie);
            List<string> lista = new List<string>();
            foreach (var x in numbers)
            {
                lista.Add(x.ToString().Substring(x.ToString().IndexOf("rozklad=")+10));
            }
            foreach (var x in lista)
            {
                LiniaComboBox.Items.Add(x);
            }
        }

        private void LiniaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KierunkiComboBox.Items.Clear();
            var kierunek = LiniaComboBox.SelectedItem;
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            htmlkierunek = webClient.DownloadString("http://www.komunikacja.bialystok.pl/?page=lista_przystankow&nr="+kierunek.ToString()+"&rozklad=");
            htmlkierunek = htmlkierunek.Substring(htmlkierunek.IndexOf("<div id=\"lista_przystankow\">"),htmlkierunek.LastIndexOf("</div><!--lista_przystankow-->")- htmlkierunek.IndexOf("<div id=\"lista_przystankow\">"));
            List<Funkcje.LinkItem> kierunki = new List<Funkcje.LinkItem>();
            kierunki = Funkcje.Findkierunek(htmlkierunek);
            listaprzystankow = null;
            listaprzystankow = new List<string>();
            string pomoc;
            foreach (var x in kierunki)
            {
                pomoc = Regex.Replace(x.ToString(), @"\t|\n|\r", "");
                listaprzystankow.Add(pomoc);
            }
            
            foreach (var x in listaprzystankow)
            {
                bool test1 = x.Substring(0, 8).Equals("kierunek");
                if (test1)
                {
                    
                    KierunkiComboBox.Items.Add(x);
                }
            }
        }

        private void KierunkiComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PrzystanekComboBox.Items.Clear();
            var linia = KierunkiComboBox.SelectedItem;
            bool poczatek = false;
            bool poczatek2 = false;
            int i = -1;
            List<Funkcje.LinkItem> numery = new List<Funkcje.LinkItem>();
            List<string> nrList = new List<string>();
            numery = Funkcje.Findlink(htmlkierunek);
            foreach (var x in numery)
            {
                nrList.Add(x.ToString().Substring(x.ToString().IndexOf("nrp=") + 4, x.ToString().IndexOf("k=") - x.ToString().IndexOf("nrp=") - 9));
            }

            foreach (var x in listaprzystankow)
            {
                if (!poczatek)
                {

                    poczatek = x.Equals(linia);
                }
                else
                {
                    if (x.Substring(0, 8) != "kierunek")
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = x;
                        item.Tag = nrList[i];
                        PrzystanekComboBox.Items.Add(item);
                    }
                    else
                    {
                        return;
                    }
                }
                i++;
                if (!poczatek2 && !poczatek && i > 0)
                {
                    i--;
                    poczatek2 = true;
                }
            }
        }

        private void DalejButton_Click(object sender, RoutedEventArgs e)
        {
            if (LiniaComboBox.SelectedItem == null || KierunkiComboBox.SelectedItem == null ||
                PrzystanekComboBox.SelectedItem == null)
            {
                InfoLabel.Content = "Wybierz wszystkie dane!";
            }
            else
            {
                ComboBoxItem nrp = (ComboBoxItem)PrzystanekComboBox.SelectedItem;
                string htmllink = "http://www.komunikacja.bialystok.pl/?page=przystanek&nrl="+LiniaComboBox.SelectedValue+"&nrp="+nrp.Tag+"&k=0&rozklad=";
                Rozklad rozklad = new Rozklad(htmllink);
                string nazwa = Regex.Replace(nrp.Content.ToString(), @"\t|\n|\r", "");
                string linia = Regex.Replace(LiniaComboBox.SelectedValue.ToString(), @"\t|\n|\r", "");
                string kierunek = Regex.Replace(KierunkiComboBox.SelectedValue.ToString(), @"\t|\n|\r", "");
                rozklad.MainLabel.Content ="Rozklad przystanku nr: " + nrp.Tag + ", " + nrp.Content + " dla linii nr: " + LiniaComboBox.SelectedValue + ", " + KierunkiComboBox.SelectedValue;
                rozklad.MainLabel.Content = "Linia nr:" + linia + Environment.NewLine + "Przystanek nr: " + nrp.Tag +
                                            ", " + nazwa + Environment.NewLine + kierunek;
                rozklad.Show();
                this.Close();
            }
        }
    }
}
