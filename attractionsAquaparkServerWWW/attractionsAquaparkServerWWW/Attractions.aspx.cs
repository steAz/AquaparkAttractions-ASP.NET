using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MassTransit;
using komunikaty;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;

namespace attractionsAquaparkServerWWW
{

    public partial class Attractions : System.Web.UI.Page
    {

        bool Logged = false;

        public static void CheckUser()
        {

        }

        public Task Handle(ConsumeContext<Komunikat> ctx)
        {
            if(ctx.Message.Tekst == "Answer Register LoginExists")
            {
                string message = "alert('" + "Konto o podanym loginie już istnieje." + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "Error_message", message, true);
            }
            else if (ctx.Message.Tekst == "Answer Auth LoginDoesntExist" || ctx.Message.Tekst == "Answer Auth Error")
            {
                string message = "alert('" + "Zły login lub hasło." + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "Error_message", message, true);
            }
            else if (ctx.Message.Tekst == "Answer Auth Success")
            {
                string message = "alert('" + "Sukces. Przejdziesz do strony z możliwością kupna biletów." + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "Error_message", message, true);
                LabelBoughtTickets.Visible = true;
                LabelTypeOfTicket.Visible = true;
                RadioButtonList1.Visible = true;
                LabelAllowances.Visible = true;
                CheckBoxList1.Visible = true;
                LabelTime.Visible = true;
                RadioButtonList2.Visible = true;
                ButtonPrice.Visible = true;
                LabelPriceInfo.Visible = true;
                LabelPrice.Visible = true;
                TicketBuyButton.Visible = true;
                LabelLogin.Visible = false;
                TextBoxLogin.Visible = false;
                LabelPass.Visible = false;
                TextBoxPass.Visible = false;
                ButtonAuth.Visible = false;
                ButtonRegister.Visible = false;
                Logged = true;
            }
            else if (ctx.Message.Tekst == "Answer Register Success")
            {
                string message = "alert('" + "Dodano konto." + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "Error_message", message, true);
            }

            return Task.FromResult(0);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!this.IsPostBack)
            {
                //initial loading
                SqlConnection c = new SqlConnection
                {
                    ConnectionString =
             "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AquaparkTicketsDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
                };

                c.Open();
                var cmd = c.CreateCommand();

                cmd.CommandText = "delete from tickets";
                cmd.ExecuteNonQuery();
                c.Close();
            }
        }

        void SendMessage(string message)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://luozycyv:HMETMyUNp2qMHslxNUihuSMWLb6NElPy@hound.rmq.cloudamqp.com/luozycyv"),
                h => { });

                sbc.ReceiveEndpoint(host, "answerqueue", ep =>
                {
                    ep.Handler<Komunikat>(Handle);
                });
            });

            bus.Start();



            var Release = new Komunikat()
            {
                Tekst = message,
                Dzien = DateTime.Now
            };
            bus.Publish<Komunikat>(Release);
            System.Threading.Thread.Sleep(3000);
            bus.Stop();
        }

        protected void Auth_Click(object sender, EventArgs e)
        {
            if (TextBoxLogin.Text == "" || TextBoxPass.Text == "")
            {
                string message = "alert('" + "Pole loginu lub hasła jest puste." + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "Error_message", message, true);
                return;
            }
            string authMessage = "Auth;" +
                    TextBoxLogin.Text + ";" +
                    TextBoxPass.Text;
            SendMessage(authMessage);

        }

        protected void Register_Click(object sender, EventArgs e)
        {
            if(TextBoxLogin.Text == "" || TextBoxPass.Text == "")
            {
                string message = "alert('" + "Pole loginu lub hasła jest puste." + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "Error_message", message, true);
                return;
            }
            string registerMessage = "Register;" +
                     TextBoxLogin.Text + ";" +
                     TextBoxPass.Text;
            SendMessage(registerMessage);
        }

        void UpdateDivTickets()
        {
            SqlConnection c = new SqlConnection
            {
                ConnectionString =
              "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AquaparkTicketsDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
            };

            c.Open();
            var cmd = c.CreateCommand();

            cmd.CommandText = "select id, type , allowances, time, totalPrice from tickets";
            var rc = cmd.ExecuteReader();
            while (rc.Read())
            {
                var ticketLabel = new Label();
                ticketLabel.Text = "<b>" + "Bilet " + rc.GetInt32(0).ToString() + "</b>" + " " + rc.GetString(1) +
                            "; z dodatkami: " + rc.GetString(2) +
                            "; na czas: " + rc.GetString(3) +
                            "; o całkowitej wartości " + rc.GetInt32(4).ToString();
                var divTickets = this.FindControl("divTickets") as HtmlGenericControl;
                divTickets.Visible = true;
                //divTickets.Style.Add(HtmlTextWriterStyle.BackgroundColor, "yellow");
                divTickets.Controls.Add(ticketLabel);
                divTickets.Controls.Add(new LiteralControl("<br />"));
                divTickets.Controls.Add(new LiteralControl("<br />"));
            }
            rc.Close();
            c.Close();
        }

        protected void BuyTicket_Click(object sender, EventArgs e)
        {
            if(LabelPrice.Text.ToString() == "")
            {
                string message = "alert('" + "Nie przeliczono ceny." + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "Error_message", message, true);
                UpdateDivTickets();
                return;
            }

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://luozycyv:HMETMyUNp2qMHslxNUihuSMWLb6NElPy@hound.rmq.cloudamqp.com/luozycyv"),
                h => { });

                sbc.ReceiveEndpoint(host, "answerqueue", ep =>
                {
                 //   ep.Consumer<KomunikatConsumer>();
                });
            });

            bus.Start();

            string ticketRelease = "Ticket;" +
                                RadioButtonList1.SelectedItem.ToString() + ";" + // type of ticket
                                RadioButtonList2.SelectedItem.ToString() + ";" + // time of ticket
                                LabelPrice.Text.ToString() + ";";

            List<ListItem> selectedItems = CheckBoxList1.Items.Cast<ListItem>()
              .Where(li => li.Selected)
                  .ToList();
            foreach (var item in selectedItems)
            {
                if (item.Text.ToString() == "Strefa saun - 8zł")
                {
                    ticketRelease += item.Text.ToString();
                }
                else if (item.Text.ToString() == "Dostęp do zjeżdżalni - 4zł")
                {
                    ticketRelease += ", " + item.Text.ToString();
                }
                else if (item.Text.ToString() == "Strefa wypoczynkowa - 8zł")
                {
                    ticketRelease += ", " + item.Text.ToString();
                }
            }

            var ticketMessage = new Komunikat()
            {
                Tekst = ticketRelease,
                Dzien = DateTime.Now
            };
            bus.Publish<Komunikat>(ticketMessage);
            System.Threading.Thread.Sleep(3000);
            bus.Stop();

            UpdateDivTickets();
            LabelPrice.Text = "";
        }

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        protected void RadioButtonList2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void CountPrice_Click(object sender, EventArgs e)
        {
            var totalPrice = 0;

            //if (RadioButtonList2.SelectedItem == RadioButtonList2.)
            if (RadioButtonList2.SelectedIndex == -1 || RadioButtonList1.SelectedIndex == -1)
            {
                string message = "alert('" + "Proszę wybrać wszystkie opcje oznaczone gwiazdką." + "');";
                ClientScript.RegisterStartupScript(this.GetType(), "Error_message", message, true);
                UpdateDivTickets();
                return;
            }

            if (RadioButtonList1.SelectedItem.ToString() == "Ulgowy - 18zł")
            {
                totalPrice += 18;
            }
            else if (RadioButtonList1.SelectedItem.ToString() == "Rodzinny na 3 osoby - 50zł")
            {
                totalPrice += 50;
            }
            else if (RadioButtonList1.SelectedItem.ToString() == "Normalny - 22zł")
            {
                totalPrice += 22;
            }

            if (RadioButtonList2.SelectedItem.ToString() == "2 godziny")
            {
                totalPrice += 10;
            }
            else if (RadioButtonList2.SelectedItem.ToString() == "3 godziny")
            {
                totalPrice += 20;
            }
            else if (RadioButtonList2.SelectedItem.ToString() == "do końca dnia")
            {
                totalPrice += 32;
            }

            List<ListItem> selectedItems = CheckBoxList1.Items.Cast<ListItem>()
                .Where(li => li.Selected)
                    .ToList();
            foreach ( var item in selectedItems)
            {
                if (item.Text.ToString() == "Strefa saun - 8zł")
                {
                    totalPrice += 8;
                }
                else if (item.Text.ToString() == "Dostęp do zjeżdżalni - 4zł")
                {
                    totalPrice += 4;
                }
                else if (item.Text.ToString() == "Strefa wypoczynkowa - 8zł")
                {
                    totalPrice += 8;
                }
            }

            LabelPrice.Text = totalPrice.ToString();
            UpdateDivTickets();
        }
    }
}