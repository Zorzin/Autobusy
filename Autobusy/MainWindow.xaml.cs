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

        private List<String> listaprzystankow;
        private string htmlkierunek;
        public struct LinkItem
        {
            public string Href;
            public string Text;

            public override string ToString()
            {
                return Href + "\n\t" + Text;
            }
        }

            public static List<LinkItem> Findlink(string file)
            {
                List<LinkItem> list = new List<LinkItem>();

                // 1.
                // Find all matches in file.
                MatchCollection m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)",
                    RegexOptions.Singleline);

                // 2.
                // Loop over each match.
                foreach (Match m in m1)
                {
                    string value = m.Groups[1].Value;
                    LinkItem i = new LinkItem();

                    // 3.
                    // Get href attribute.
                    Match m2 = Regex.Match(value, @"href=\""(.*?)\""",
                    RegexOptions.Singleline);
                    if (m2.Success)
                    {
                        i.Href = m2.Groups[1].Value;
                    }

                    // 4.
                    // Remove inner tags from text.
                    string t = Regex.Replace(value, @"\s*<.*?>\s*", "",
                    RegexOptions.Singleline);
                    i.Text = t;

                    list.Add(i);
                }
                return list;
            }

        public static List<LinkItem> Findkierunek(string file)
        {
            List<LinkItem> list = new List<LinkItem>();

            // 1.
            // Find all matches in file.
            MatchCollection m1 = Regex.Matches(file, @"(<td.*?>.*?</td>)",
                RegexOptions.Singleline);

            // 2.
            // Loop over each match.
            foreach (Match m in m1)
            {
                string value = m.Groups[1].Value;
                LinkItem i = new LinkItem();

                // 3.
                // Get href attribute.
                Match m2 = Regex.Match(value, @"kierunek:\""(.*?)\""",
                RegexOptions.Singleline);
                if (m2.Success)
                {
                    i.Href = m2.Groups[1].Value;
                }

                // 4.
                // Remove inner tags from text.
                string t = Regex.Replace(value, @"\s*<.*?>\s*", "",
                RegexOptions.Singleline);
                i.Text = t;

                list.Add(i);
            }
            return list;
        }

        public MainWindow()
        {
            InitializeComponent();
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string html = client.DownloadString("http://www.komunikacja.bialystok.pl/?page=rozklad_jazdy");

            string linie = html.Substring(html.IndexOf("<!-- START box: tabela_linii -->"),
                html.LastIndexOf("<!-- END box: tabela_linii -->")- html.IndexOf("<!-- START box: tabela_linii -->"));

            List<LinkItem> numbers = new List<LinkItem>();
            numbers = Findlink(linie);
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
            List<LinkItem> kierunki = new List<LinkItem>();
            kierunki = Findkierunek(htmlkierunek);
            listaprzystankow = null;
            listaprzystankow = new List<string>();
            foreach (var x in kierunki)
            {
                listaprzystankow.Add(x.ToString());
            }
            foreach (var x in listaprzystankow)
            {
                bool test1 = x.Substring(2, 8).Equals("kierunek");
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
            bool poczatek=false;
            foreach (var x in listaprzystankow)
            {
                if (!poczatek)
                {
                    poczatek = x.Equals(linia);
                }
                else
                {
                    if (x.Substring(2,8)!="kierunek")
                    {
                        PrzystanekComboBox.Items.Add(x);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }
}
