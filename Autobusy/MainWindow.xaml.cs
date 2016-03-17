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
        public struct LinkItem
        {
            public string Href;
            public string Text;

            public override string ToString()
            {
                return Href + "\n\t" + Text;
            }
        }

            public static List<LinkItem> Find(string file)
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

        public MainWindow()
        {
            InitializeComponent();
            WebClient client = new WebClient();
            string html = client.DownloadString("http://www.komunikacja.bialystok.pl/?page=rozklad_jazdy");

            string linie = html.Substring(html.IndexOf("<!-- START box: tabela_linii -->"),
                html.LastIndexOf("<!-- END box: tabela_linii -->")- html.IndexOf("<!-- START box: tabela_linii -->"));

            List<LinkItem> numbers = new List<LinkItem>();
            numbers = Find(linie);
            List<string> lista = new List<string>();
            foreach (var x in numbers)
            {
                lista.Add(x.ToString().Substring(x.ToString().IndexOf("rozklad=")+10));
            }
            foreach (var x in lista)
            {
                LiniaComboBox.Items.Add(x);
            }
            
            Console.WriteLine(linie);
        }

        private void LiniaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBoxItem kierunek = (ComboBoxItem)LiniaComboBox.SelectedItem;
            WebClient webClient = new WebClient();
            string htmlkierunek = webClient.DownloadString("http://www.komunikacja.bialystok.pl/?page=lista_przystankow&nr="+kierunek.Content.ToString()+"&rozklad=");

        }
    }
}
