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
        private Dictionary<string, List<Message>> Groups = new Dictionary<string,List<Message>>();
        public string aktuelleGruppe;
        private string userId;
        public MainWindow()
        {
            InitializeComponent();
            OnStartUp();

            chatHubProxy.On<Message>("GroupEmpfange", (msg) => Dispatcher.Invoke(() =>
            {
            
                aktuelleGruppe = msg.groupname;
                var list = Groups[aktuelleGruppe]; //list rausholen und text hinzufügen
                list.Add(msg);
                Groups[aktuelleGruppe] = list;
                scrollBar.ScrollToBottom(); //scrollbar springt auf letztes Element
                if (GroupName.Text == aktuelleGruppe)
                {
                    if (userId == msg.connID)
                    {
                        tbSendNachrichten.Text += msg.ToString();
                        tbEmpfNachrichten.Text += "\n\n\n";
                    }
                    else
                    {
                        tbEmpfNachrichten.Text += msg.ToString();
                        tbSendNachrichten.Text += "\n\n\n";
                    }
                }
                else //wenn user andere Gruppe ausgewählt hat
                {
                    //notification
                    if (userId != msg.connID) //wenn Nachricht nicht selbst verschickt wird
                    {
                        new ToastContentBuilder()
                            .AddArgument("action", "viewConversation")
                            .AddText($"{msg.groupname}")
                            .AddText($"{msg.sender}: {msg.text}")
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
            Groups.Add("Nickgür", new List<Message>());
            Groups.Add("Drilon", new List<Message>());
            Groups.Add("Nick", new List<Message>());
            Groups.Add("Tim", new List<Message>());
            Groups.Add("Alice", new List<Message>());
            Groups.Add("Bob", new List<Message>());
            Groups.Add("Moooooooooin", new List<Message>());
            Groups.Add("WI20Z1A", new List<Message>());
            Groups.Add("X Æ A-12", new List<Message>());

            foreach (var gruppe in Groups.Keys.ToList())
            {
               chatHubProxy.Invoke("AddToGroup", gruppe);
            }
        }

        private void Senden_Click(object sender, RoutedEventArgs e)
        {
            if (tbSendeNachricht.Text != "") // Nur wenn Text geschrieben
            {
                Message msg = new Message(tbSendeNachricht.Text, DateTime.Now, tbusername.Text, aktuelleGruppe, userId);
                chatHubProxy.Invoke("SendMessageToGroup", msg);
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
                    if (message.connID == userId) //gesendete rechts und empfangene links
                    {
                        tbSendNachrichten.Text += message.ToString();
                        tbEmpfNachrichten.Text += "\n\n\n";
                    }
                    else
                    {
                        tbEmpfNachrichten.Text += message.ToString();
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
                Groups.Add(add, new List<Message>());
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

