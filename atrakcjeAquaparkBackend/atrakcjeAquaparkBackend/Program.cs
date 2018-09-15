using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using komunikaty;
using MassTransit;
using System.Data.SqlClient;

namespace atrakcjeAquaparkBackend
{
    class Handler : IConsumer<IKomunikat>
    {

        public Task Consume(ConsumeContext<IKomunikat> context)
        {
            Console.WriteLine(context.Message.Tekst + " " + context.Message.Dzien + " " + System.Threading.Thread.CurrentThread.ManagedThreadId);

            SqlConnection c = new SqlConnection
            {
                ConnectionString =
                 "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AquaparkTicketsDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
            };

            c.Open();
            var cmd = c.CreateCommand();
            cmd.CommandText = "select count(id) from tickets";
            int numOfIDs = (Int32)cmd.ExecuteScalar();
            cmd.CommandText = "select count(id) from users";
            int numOfUsers = (Int32)cmd.ExecuteScalar();



            if (context.Message.Tekst.StartsWith("Ticket"))
            {
                var commandArray = context.Message.Tekst.Split(';');

                var numOfTicket = numOfIDs + 1;
                cmd.CommandText = "insert into tickets (id, type, allowances, time, totalPrice) values('" + numOfTicket + 
                                                                                                    "', '" + commandArray[1] + 
                                                                                                    "', '" + commandArray[4] + 
                                                                                                    "', '" + commandArray[2] + 
                                                                                                    "', '" + commandArray[3] + "')";                                                                          
                cmd.ExecuteNonQuery();
            }

            if (context.Message.Tekst.StartsWith("Register"))
            {
                var commandArray = context.Message.Tekst.Split(';');

                cmd.CommandText = "select login, pass from users where login ='" + commandArray[1] + "'";
                var rc = cmd.ExecuteReader();

                string login = "";
                string pass = "";
                while (rc.Read())
                {
                    login = rc.GetString(0);
                    pass = rc.GetString(1);
                }
                rc.Close();

                if (login=="")
                {
                   
                    var numOfUser = numOfUsers + 1;
                    cmd.CommandText = "insert into users (id, login, pass) values('" + numOfUser +
                                                                                    "', '" + commandArray[1] +
                                                                                    "', '" + commandArray[2] + "')";
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("Register Success");
                    context.RespondAsync<Komunikat>(new Komunikat()
                    {
                        Tekst = "Answer Register Success",
                        Dzien = DateTime.Now
                    });
                } else
                {
                    Console.WriteLine("Register LoginExists");
                    context.RespondAsync<Komunikat>(new Komunikat()
                    {
                        Tekst = "Answer Register LoginExists",
                        Dzien = DateTime.Now
                    });
                }
            }

            if (context.Message.Tekst.StartsWith("Auth"))
            {
                var commandArray = context.Message.Tekst.Split(';');

                cmd.CommandText = "select login, pass from users where login ='" + commandArray[1] + "'";
                var rc = cmd.ExecuteReader();

                string login = "";
                string pass = "";
                while (rc.Read())
                {
                    login = rc.GetString(0);
                    pass = rc.GetString(1);
                }
                rc.Close();

                if (login == "") // jeśli nie ma takiego loginu
                {
                    Console.WriteLine("Auth LoginDoesntExist");
                    context.RespondAsync<Komunikat>(new Komunikat()
                    {
                        Tekst = "Answer Auth LoginDoesntExist",
                        Dzien = DateTime.Now
                    });
                }
                else if (pass != commandArray[2])
                {
                    Console.WriteLine("Auth Error");
                    context.RespondAsync<Komunikat>(new Komunikat()
                    {
                        Tekst = "Answer Auth Error",
                        Dzien = DateTime.Now
                    });
                }
                else
                {
                    Console.WriteLine("Auth Success");
                    context.RespondAsync<Komunikat>(new Komunikat()
                    {
                        Tekst = "Answer Auth Success",
                        Dzien = DateTime.Now
                    });
                }
            }
                c.Close();


            
            return Task.FromResult(0);
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://luozycyv:HMETMyUNp2qMHslxNUihuSMWLb6NElPy@hound.rmq.cloudamqp.com/luozycyv"),
                h => { });
                sbc.ReceiveEndpoint(host, "recvqueue", ep =>
                {
                    ep.Consumer<Handler>();
                    //ep.Instance(instancja);
                    // ep.Consumer(Fabryka);
                });
            });
            bus.Start();
            Console.WriteLine("Abonent started");
            Console.ReadKey();
            bus.Stop();
        }
    }
}
