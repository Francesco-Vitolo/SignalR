using Microsoft.AspNet.SignalR.Client;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Windows.UI.ViewManagement.Core;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection hubConnection = new HubConnection("http://localhost:56871/");
        IHubProxy chatHubProxy;
        private Dictionary<string, List<string>> Groups = new Dictionary<string,List<string>>();
        public string aktuelleGruppe;
        private string userId;
        public MainWindow()
        {
            InitializeComponent();
            OnStartUp();

            chatHubProxy.On<string, string, string>("GroupEmpfange", (text, groupName, senderID) => Dispatcher.Invoke(() =>
            {
                aktuelleGruppe = groupName;
                var v = Groups[aktuelleGruppe]; //list rausholen und text hinzufügen
                v.Add(text);
                Groups[aktuelleGruppe] = v;
                scrollBar.ScrollToBottom(); //scrollbar springt auf letztes Element
                if (GroupName.Text == aktuelleGruppe)
                {
                    if (userId == senderID)
                    {
                        tbSendNachrichten.Text += "\n" + text;
                        tbEmpfNachrichten.Text += "\n\n\n";
                    }
                    else
                    {
                        tbEmpfNachrichten.Text += "\n" + text;
                        tbSendNachrichten.Text += "\n\n\n";
                    }
                }
                else
                {
                    //notification
                    if (userId != senderID) //wenn Nachricht nicht selbst verschickt wird
                    {
                        new ToastContentBuilder()
                            .AddArgument("action", "viewConversation")
                            .AddText($"{aktuelleGruppe}")
                            .AddText($"{text.Substring(0,text.Length-5)}")
                            .Show();
                    }
                }
            }));
        }

        private void OnStartUp()
        {
            lv.DataContext = Groups.Keys; //Listview
            tbusername.Text = Environment.UserName; //Windows - Name
            chatHubProxy = hubConnection.CreateHubProxy("ChatHub");
            hubConnection.Start().Wait();
            userId = hubConnection.ConnectionId; //connID
            AddToGroups();
            aktuelleGruppe = Groups.Keys.First(); //Für Listview
            GroupName.Text = aktuelleGruppe;
        }
        private void AddToGroups()
        {
            Groups.Add("Nickgür", new List<string>());
            Groups.Add("Drilon", new List<string>());
            Groups.Add("Nick", new List<string>());
            Groups.Add("Tim", new List<string>());
            Groups.Add("Alice", new List<string>());
            Groups.Add("Bob", new List<string>());
            Groups.Add("Moooooooooin", new List<string>());
            Groups.Add("WI20Z1A", new List<string>());
            Groups.Add("X Æ A-12", new List<string>());

            foreach (var gruppe in Groups.Keys.ToList())
            {
               chatHubProxy.Invoke("AddToGroup", gruppe);
            }
        }

        private void Senden_Click(object sender, RoutedEventArgs e)
        {
            if (tbSendeNachricht.Text != "") // Nur wenn Text geschrieben
            {
                chatHubProxy.Invoke("SendMessageToGroup", aktuelleGruppe, tbSendeNachricht.Text, tbusername.Text);
                tbSendeNachricht.Text = "";
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListView lv = (ListView)sender;
            aktuelleGruppe = Convert.ToString(lv.SelectedItem);
            GroupName.Text = aktuelleGruppe;
            tbEmpfNachrichten.Text = "";
            tbSendNachrichten.Text = "";
            if (Groups.Count() != 0)
            {
                foreach (var message in Groups[aktuelleGruppe])
                {
                    if (message.Contains(userId.Substring(0, 5))) //gesendete rechts und empfangene links
                    {
                        tbSendNachrichten.Text += "\n" + message;
                        tbEmpfNachrichten.Text += "\n\n\n";
                    }
                    else
                    {
                        tbEmpfNachrichten.Text += "\n" + message;
                        tbSendNachrichten.Text += "\n\n\n";
                    }
                }
            }
        }
        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            //Button b = (Button)sender;
            //b.IsEnabled = false;
            string add = tbNewGroup.Text;
            chatHubProxy.Invoke("AddToGroup", add).Wait();
            if (Groups.Keys.All(x => x != add)) //abfangen selber Gruppenname
            {
                Groups.Add(add, new List<string>());
                lv.DataContext = null;         //reset listview
                lv.DataContext = Groups.Keys;
                MessageBox.Show($"Gruppe {add} hinzugefügt");
            }
            else
            {
                MessageBox.Show("Gruppe schon vorhanden");
            }
        }

        private void lv_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            chatHubProxy.Invoke("RemoveGroup", aktuelleGruppe);
            ListView_MouseDoubleClick(lv, null); //des gewünschte Objekt wird ausgewählt
            Groups.Remove(aktuelleGruppe);
            lv.SelectedIndex = 0;
            ListView_MouseDoubleClick(lv, null); // springt auf die erste Gruppe
            lv.DataContext = null;         //reset listview
            lv.DataContext = Groups.Keys;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Senden_Click(null, null);
        }

        private void ButtonEmoji_Click(object sender, RoutedEventArgs e)
        {
            CoreInputView.GetForCurrentView().TryShow(CoreInputViewKind.Emoji);
            tbSendeNachricht.Focus();
        }       
    }
}

