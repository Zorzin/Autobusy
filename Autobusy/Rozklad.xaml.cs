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
        public Rozklad(string html)
        {
            InitializeComponent();
            WebClient client = new WebClient();
            client.Encoding= Encoding.UTF8;
            string strona = client.DownloadString(html);
            strona = strona.Substring(strona.IndexOf("id=\"rozklad_1\""), strona.LastIndexOf("</table>")- strona.IndexOf("id=\"rozklad_1\""));
            List<Funkcje.LinkItem> lista = new List<Funkcje.LinkItem>();
            lista = Funkcje.Findkierunek(strona);
            Console.WriteLine("asdfsadfs");
        }
    }
}
