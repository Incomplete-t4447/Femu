using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SAHB.GraphQL;
using SAHB.GraphQLClient;
using SAHB.GraphQLClient.Extentions;
using SAHB.GraphQLClient.FieldBuilder.Attributes;
using SAHB.GraphQLClient.QueryGenerator;
using System.Windows;

namespace FELM
{



    public class API
    {
        const string APIIP = "http://192.168.76.20:8080/";
        IGraphQLHttpClient client = GraphQLHttpClient.Default();
        public async Task<JObject> LoginQueryAsync(string userName, string password)
        {
            using (SHA1 sha1Hash = SHA1.Create())
            {
                try
                {
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
                    byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                    string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    Console.WriteLine(hashedPassword);

                    var query = client.CreateQuery(builder =>
                         builder.Field("verifyLogin",
                            verifyLogin =>
                                verifyLogin
                                    .Argument("username", "String", "username", true)
                                    .Argument("password", "String", "password", true)
                                    .Field("status")
                                    .Field("type")
                                    ),
                                    APIIP, arguments: new[] { new GraphQLQueryArgument("username", userName), new GraphQLQueryArgument("password", hashedPassword) });
                    var response = await query.Execute();
                    JObject result = response["verifyLogin"];

                    return result;
                }
                catch
                {
                    
                    MessageBoxResult messageResult = MessageBox.Show("Der var et problem med forbindelsen til serveren", "Server fejl", MessageBoxButton.OK);
                    if(messageResult == MessageBoxResult.OK)
                    {
                        Globalvar.NoNet = true;
                        Application.Current.Shutdown();
                    }

                    JObject result = new JObject();
                    return result;
                }
                //From String to byte array
                
                // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
            }
        }

        public async Task<JArray> getUsers()
        {
            
            var query = client.CreateQuery(builder =>
                builder.Field("User",
                    GetUsers =>
                        GetUsers
                            .Field("username")
                            .Field("name")
                            .Field("password")
                            .Field("color")
                            .Field("type")
                            .Field("adresse")
                            .Field("husnr")
                            .Field("postnr")
                            .Field("tlfnr")
                            .Field("event")
                            .Field("mail")
                            .Field("rfidNummer")
                            ),
                            APIIP);
            var response = await query.Execute();
            JArray result = response["User"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> CreateUserAsync(
                string username, 
                string name, 
                string password, 
                string mail, 
                string color, 
                string type, 
                string adresse, 
                string nr, 
                int postnr, 
                int telefonnummer,
                string rfidnummer
            )
        {
            using (SHA1 sha1Hash = SHA1.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                var query = client.CreateQuery(builder =>
                builder.Field("createUser",
                    createEkstra =>
                        createEkstra
                            .Argument("username", "string", "username", true)
                            .Argument("name", "string", "name", true)
                            .Argument("password", "string", "password", true)
                            .Argument("mail", "string", "mail", true)
                            .Argument("color", "string", "color", true)
                            .Argument("type", "string", "type", true)
                            .Argument("adresse", "string", "adresse", true)
                            .Argument("husnr", "string", "husnr", true)
                            .Argument("postnr", "int", "postnr", true)
                            .Argument("tlfnr", "int", "tlfnr", true)
                            .Argument("rfidnummer", "string", "rfidnummer", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("username", username),
                                new GraphQLQueryArgument("name", name),
                                new GraphQLQueryArgument("password", hashedPassword),
                                new GraphQLQueryArgument("mail", mail),
                                new GraphQLQueryArgument("color", color),
                                new GraphQLQueryArgument("type", type),
                                new GraphQLQueryArgument("adresse", adresse),
                                new GraphQLQueryArgument("husnr", nr),
                                new GraphQLQueryArgument("postnr", postnr),
                                new GraphQLQueryArgument("tlfnr", telefonnummer),
                                new GraphQLQueryArgument("rfidnummer", rfidnummer),
                            });
                var response = await query.Execute();
                JArray result = response["createUser"];
                return result;
                // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
            }
        }

        public async Task<JArray> EditUserAsync(
                string username,
                string newUsername,
                string name,
                string mail,
                string color,
                string type,
                string adresse,
                string nr,
                int postnr,
                int telefonnummer,
                string rfidnummer,
                string password
            )
        {
            if(password == "")
            {
                var query = client.CreateQuery(builder =>
                builder.Field("editUser",
                    createEkstra =>
                        createEkstra
                            .Argument("username", "string", "username", true)
                            .Argument("newUsername", "string", "newUsername", true)
                            .Argument("name", "string", "name", true)
                            .Argument("mail", "string", "mail", true)
                            .Argument("color", "string", "color", true)
                            .Argument("type", "string", "type", true)
                            .Argument("adresse", "string", "adresse", true)
                            .Argument("husnr", "string", "husnr", true)
                            .Argument("postnr", "int", "postnr", true)
                            .Argument("tlfnr", "int", "tlfnr", true)
                            .Argument("rfidnummer", "string", "rfidnummer", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("username", username),
                                new GraphQLQueryArgument("newUsername", newUsername),
                                new GraphQLQueryArgument("name", name),
                                new GraphQLQueryArgument("mail", mail),
                                new GraphQLQueryArgument("color", color),
                                new GraphQLQueryArgument("type", type),
                                new GraphQLQueryArgument("adresse", adresse),
                                new GraphQLQueryArgument("husnr", nr),
                                new GraphQLQueryArgument("postnr", postnr),
                                new GraphQLQueryArgument("tlfnr", telefonnummer),
                                new GraphQLQueryArgument("rfidnummer", rfidnummer)
                            });
                var response = await query.Execute();
                JArray result = response["editUser"];
                return result;
                // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
            } else if (password != "")
            {
                using (SHA1 sha1Hash = SHA1.Create())
                {
                    //From String to byte array
                    byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
                    byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                    string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    var query = client.CreateQuery(builder =>
                builder.Field("editUser",
                    createEkstra =>
                        createEkstra
                            .Argument("username", "string", "username", true)
                            .Argument("newUsername", "string", "newUsername", true)
                            .Argument("name", "string", "name", true)
                            .Argument("mail", "string", "mail", true)
                            .Argument("color", "string", "color", true)
                            .Argument("type", "string", "type", true)
                            .Argument("adresse", "string", "adresse", true)
                            .Argument("husnr", "string", "husnr", true)
                            .Argument("postnr", "int", "postnr", true)
                            .Argument("tlfnr", "int", "tlfnr", true)
                            .Argument("password", "string", "password", true)
                            .Argument("rfidnummer", "string", "rfidnummer", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("username", username),
                                new GraphQLQueryArgument("newUsername", newUsername),
                                new GraphQLQueryArgument("name", name),
                                new GraphQLQueryArgument("mail", mail),
                                new GraphQLQueryArgument("color", color),
                                new GraphQLQueryArgument("type", type),
                                new GraphQLQueryArgument("adresse", adresse),
                                new GraphQLQueryArgument("husnr", nr),
                                new GraphQLQueryArgument("postnr", postnr),
                                new GraphQLQueryArgument("tlfnr", telefonnummer),
                                new GraphQLQueryArgument("password", hashedPassword),
                                new GraphQLQueryArgument("rfidnummer", rfidnummer)
                            });
                    var response = await query.Execute();
                    JArray result = response["editUser"];
                    return result;
                    // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";

                }
            }
            return null;
        }

        public async Task<JArray> DeleteUserAsync(
                string username
            )
        {
            var query = client.CreateQuery(builder =>
                builder.Field("deleteUser",
                    createEkstra =>
                        createEkstra
                            .Argument("username", "string", "username", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("username", username)
                            });
            var response = await query.Execute();
            JArray result = response["deleteUser"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> createLokation(
               string lokation,
               string description,
               int VareNrStart,
               int VareNrSlut
           )
        {

            var query = client.CreateQuery(builder =>
            builder.Field("createLokation",
                CreateEventUser =>
                    CreateEventUser
                        .Argument("location", "string", "location", true)
                        .Argument("description", "string", "description", true)
                        .Argument("VareNrStart", "int", "VareNrStart", true)
                        .Argument("VareNrSlut", "int", "VareNrSlut", true)
                        ),
                        APIIP, arguments: new[] {
                            new GraphQLQueryArgument("location", lokation),
                            new GraphQLQueryArgument("description", description),
                            new GraphQLQueryArgument("VareNrStart", VareNrStart),
                            new GraphQLQueryArgument("VareNrSlut", VareNrSlut)
                        });
            var response = await query.Execute();
            JArray result = response["createLokation"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";

        }

        public async Task<JArray> editlokation(
            string location,
            string description,
            int VareNrStart,
            int VareNrSlut)
        {
            var query = client.CreateQuery(builder =>
                builder.Field("editlokation",
                    editlokation =>
                        editlokation
                            .Argument("location", "string", "location", true)
                            .Argument("description", "string", "description", true)
                            .Argument("VareNrStart", "int", "VareNrStart", true)
                            .Argument("VareNrSlut", "int", "VareNrSlut", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("location", location),
                                new GraphQLQueryArgument("description", description),
                                new GraphQLQueryArgument("VareNrStart", VareNrStart),
                                new GraphQLQueryArgument("VareNrSlut", VareNrSlut)
                            });
            var response = await query.Execute();
            JArray result = response["editlokation"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> updatePrice(
            string WebshopNummer,
            string Salgspris)
        {
            var query = client.CreateQuery(builder =>
                builder.Field("updatePrice",
                    updatePrice =>
                        updatePrice
                            .Argument("WebshopNummer", "string", "WebshopNummer", true)
                            .Argument("Salgspris", "string", "Salgspris", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("WebshopNummer", WebshopNummer),
                                new GraphQLQueryArgument("Salgspris", Salgspris)
                            });
            var response = await query.Execute();
            JArray result = response["updatePrice"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> getLokations()
        {
            var query = client.CreateQuery(builder =>
                builder.Field("lokation",
                    GetEvents =>
                        GetEvents
                            .Field("LocationdID")
                            .Field("location")
                            .Field("description")
                            .Field("VareNrStart")
                            .Field("VareNrSlut")),
                            APIIP);
            var response = await query.Execute();
            JArray result = response["lokation"];

            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> GetEventUsers()
        {
            var query = client.CreateQuery(builder =>
                builder.Field("GetEventUsers",
                    GetEvents =>
                        GetEvents
                            .Field("Id")
                            .Field("Username")
                            .Field("EventName")),
                            APIIP);
            var response = await query.Execute();
            JArray result = response["GetEventUsers"];

            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }


        public async Task<JArray> getEvents()
        {
            
                var query = client.CreateQuery(builder =>
                builder.Field("Event",
                    GetEvents =>
                        GetEvents
                            .Field("EventName")
                            .Field("EventId")
                            .Field("EventAktiv")
                            .Field("EventBrugerId")),
                            APIIP);
                var response = await query.Execute();
                JArray result = response["Event"];
                

                return result;
            

            
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> getAllEventsAsync()
        {
            var query = client.CreateQuery(builder =>
                builder.Field("Event",
                    GetEvents =>
                        GetEvents
                            .Field("EventId")
                            .Field("EventName")
                            .Field("EventStartDato")
                            .Field("EventSlutDato")
                            .Field("EventNote")
                            .Field("EventDieselStart")
                            .Field("EventDieselSlut")
                            .Field("EventDefekt")
                            .Field("EventTlf")
                            .Field("EventAktiv")
                            .Field("EventBrugerId")),
                            APIIP);
            var response = await query.Execute();
            JArray result = response["Event"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> GetItemsQueryAsync(int num)
        {
            var query = client.CreateQuery(builder =>
                builder.Field("Vare",
                    GetAllItems =>
                        GetAllItems
                            .Argument("EventId", "int", "EventId", true)
                            .Field("varenummer")
                            .Field("beskrivelse")
                            .Field("tilgang")
                            .Field("afgang")
                            .Field("ampere")
                            .Field("note")
                            .Field("status")
                            .Field("antal")
                            .Field("vareLokation")
                            .Field("pinNr")
                            .Field("webshopNummer")
                            .Field("length")
                            .Field("rfidNummer")
                            .Field("qrKode")
                            .Field("salgspris")
                            .Field("eventVareStatus")
                            .Field("eventVareId")
                            .Field("eventVareLokation")
                            ),
                            APIIP, arguments: new[] { new GraphQLQueryArgument("EventId", num) });
            var response = await query.Execute();
            JArray result = response["Vare"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JObject> qrCode(string qrKode)
        {
            var query = client.CreateQuery(builder =>
                builder.Field("verifyQr",
                    qrCode =>
                        qrCode
                            .Argument("qrkode", "string", "qrkode", true)
                            .Field("status")
                            ),
                            APIIP, arguments: new[] { new GraphQLQueryArgument("qrkode", qrKode) });
            var response = await query.Execute();
            JObject result = response["verifyQr"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> getEventUser(
               string username
            )
        {

            var query = client.CreateQuery(builder =>
            builder.Field("getEventUser",
                GetEventUser =>
                    GetEventUser
                        .Argument("username", "string", "username", true)
                        .Field("event")
                        ),
                        APIIP, arguments: new[] {
                            new GraphQLQueryArgument("username", username)
                        });
            var response = await query.Execute();
            JArray result = response["getEventUser"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";

        }

        public async Task<JArray> createEventUser(
               string username,
               string eventName
           )
        {

            var query = client.CreateQuery(builder =>
            builder.Field("createEventUser",
                CreateEventUser =>
                    CreateEventUser
                        .Argument("username", "string", "username", true)
                        .Argument("eventName", "string", "eventName", true)
                        ),
                        APIIP, arguments: new[] {
                            new GraphQLQueryArgument("username", username),
                            new GraphQLQueryArgument("eventName", eventName)
                        });
            var response = await query.Execute();
            JArray result = response["createEventUser"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";

        }

        public async Task<JArray> deleteEventUser(
            string username
            )
        {
            var query = client.CreateQuery(builder =>
                builder.Field("deleteEventUser",
                    deleteEventUser =>
                        deleteEventUser
                            .Argument("username", "string", "username", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("username", username)
                            });
            var response = await query.Execute();
            JArray result = response["deleteEventUser"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> CreateUserAsync(
                string username,
                string name,
                string password,
                string mail,
                string color,
                string type,
                string adresse,
                string nr,
                int postnr,
                int telefonnummer
            )
        {
            using (SHA1 sha1Hash = SHA1.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                var query = client.CreateQuery(builder =>
                builder.Field("createUser",
                    createEkstra =>
                        createEkstra
                            .Argument("username", "string", "username", true)
                            .Argument("name", "string", "name", true)
                            .Argument("password", "string", "password", true)
                            .Argument("mail", "string", "mail", true)
                            .Argument("color", "string", "color", true)
                            .Argument("type", "string", "type", true)
                            .Argument("adresse", "string", "adresse", true)
                            .Argument("husnr", "string", "husnr", true)
                            .Argument("postnr", "int", "postnr", true)
                            .Argument("tlfnr", "int", "tlfnr", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("username", username),
                                new GraphQLQueryArgument("name", name),
                                new GraphQLQueryArgument("password", hashedPassword),
                                new GraphQLQueryArgument("mail", mail),
                                new GraphQLQueryArgument("color", color),
                                new GraphQLQueryArgument("type", type),
                                new GraphQLQueryArgument("adresse", adresse),
                                new GraphQLQueryArgument("husnr", nr),
                                new GraphQLQueryArgument("postnr", postnr),
                                new GraphQLQueryArgument("tlfnr", telefonnummer)
                            });
                var response = await query.Execute();
                JArray result = response["createUser"];
                return result;
                // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
            }
        }

        public async Task<string> SetFavoritQueryAsync(string Event, string Vare, string Antal)
        {
            var query = client.CreateQuery(builder =>
                builder.Field("Vare",
                    SetFavorit =>
                        SetFavorit
                            .Argument("Event", "String", "Event", true)
                            .Argument("Vare", "String", "Vare", true)
                            .Argument("Antal", "String", "Antal", true)
                            ),
                            APIIP, arguments: new[]
                            {   new GraphQLQueryArgument("Event", Event),
                                new GraphQLQueryArgument("Vare", Vare),
                                new GraphQLQueryArgument("Antal", Antal)
                            });

            var response = await query.Execute();
            string result = response["Vare"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }


        public async Task<JArray> AllEventsQueryAsync()
        {
            var query = client.CreateQuery(builder => builder.Field("Event",
                    GetAllEvents =>
                        GetAllEvents
                            .Field("EventName")
                            .Field("EventId")

                            ),
                            APIIP);
            var response = await query.Execute();
            JArray result = response["Event"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> AllEventsListQueryAsync()
        {
            var query = client.CreateQuery(builder => builder.Field("Event",
                    GetAllEvents =>
                        GetAllEvents
                            .Field("EventId")
                            .Field("EventName")
                            .Field("EventStartDato")
                            .Field("EventSlutDato")
                            .Field("EventNote")
                            .Field("EventDieselStart")
                            .Field("EventDieselSlut")
                            .Field("EventDefekt")
                            .Field("EventTlf")
                            .Field("EventAktiv")
                            .Field("EventBrugerId")
                            ),
                            APIIP);
            var response = await query.Execute();
            JArray result = response["Event"];
            List<object> EventList = new List<object>();

            var jObjects = result.ToObject<List<JObject>>();

            foreach (var obj in jObjects)
            {
                foreach (var prop in obj.Properties())
                {
                    var value = prop.Value.ToString();
                    var name = prop.Name.ToString();
                    //if (prop.Name == "EventStartDato" && string.IsNullOrEmpty(value))
                    //    obj["EventStartDato"] = "Ingen Værdi";

                    //if (prop.Name == "EventSlutDato" && string.IsNullOrEmpty(value))
                    //    obj["EventSlutDato"] = "Ingen Værdi";

                    if (string.IsNullOrEmpty(value))
                        obj[name] = "";

                }
            }
            //JArray jadsson = jObjects;
            //var json = JsonConvert.SerializeObject(new { operations = EventList });
            string json = JsonConvert.SerializeObject(jObjects,Formatting.Indented);
            JArray final = JArray.Parse(json);
            return final;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> AllEventsExportQueryAsync()
        {
            var query = client.CreateQuery(builder => builder.Field("Event",
                    GetAllEvents =>
                        GetAllEvents
                            .Field("EventName")
                            .Field("EventId")
                            ),
                            APIIP);
            var response = await query.Execute();
            JArray result = response["Event"];
            return result;
        }

        public async Task<JArray> EventAndVareQueryAsync(int idInt)
        {
            var query = client.CreateQuery(builder => builder.Field("exportVare",
                    GetAllEvent =>
                        GetAllEvent
                            .Argument("EventId", "Int", "EventId", true)
                            .Field("varenummer")
                            .Field("webshopNummer")
                            .Field("EventStartDato")
                            .Field("EventSlutDato")
                            .Field("eventVareCount")
                            ),
                            APIIP, arguments: new[] { new GraphQLQueryArgument("EventId", idInt)});
            var response = await query.Execute();
            JArray result = response["exportVare"];
                return result;
        }

        public async Task<JArray> AllFavoritsQuery()
        {
            var query = client.CreateQuery(builder => builder.Field("Vare",
                        GetAllFavorits =>
                            GetAllFavorits
                                .Argument("favorit", "int", "favorit", true)
                                .Field("beskrivelse")
                                ),
                                APIIP, arguments: new[] { new GraphQLQueryArgument("favorit", 1) });
            var response = await query.Execute();
            JArray result = response["Vare"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }


        public async Task<JArray> CreateVareAsync(
            string varenummer,
            string beskrivelse,
            string tilgang,
            string afgang,
            string ampere,
            string status,
            int antal,
            string varelokation,
            string pinNr,
            int lengde,
            string note,
            string webshopNummer,
            string RFidNummer,
            string qrKode,
            string salgspris)
        {
            var query = client.CreateQuery(builder =>
                builder.Field("createVare",
                    CreateVare =>
                        CreateVare
                            .Argument("varenummer", "string", "varenummer", true)
                            .Argument("beskrivelse", "string", "beskrivelse", true)
                            .Argument("tilgang", "string", "tilgang", true)
                            .Argument("afgang", "string", "afgang", true)
                            .Argument("ampere", "string", "ampere", true)
                            .Argument("status", "string", "status", true)
                            .Argument("antal", "int", "antal", true)
                            .Argument("varelokation", "string", "varelokation", true)
                            .Argument("pinNr", "string", "pinNr", true)
                            .Argument("lengde", "int", "lengde", true)
                            .Argument("note", "string", "note", true)
                            .Argument("webshopNummer", "string", "webshopNummer", true)
                            .Argument("RFidNummer", "string", "RFidNummer", true)
                            .Argument("qrKode", "string", "qrKode", true)
                            .Argument("salgspris", "string", "salgspris", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("varenummer", varenummer),
                                new GraphQLQueryArgument("beskrivelse", beskrivelse),
                                new GraphQLQueryArgument("tilgang", tilgang),
                                new GraphQLQueryArgument("afgang",  afgang),
                                new GraphQLQueryArgument("ampere", ampere),
                                new GraphQLQueryArgument("status", status),
                                new GraphQLQueryArgument("antal", antal),
                                new GraphQLQueryArgument("varelokation", varelokation),
                                new GraphQLQueryArgument("pinNr", pinNr),
                                new GraphQLQueryArgument("lengde", lengde),
                                new GraphQLQueryArgument("note", note),
                                new GraphQLQueryArgument("webshopNummer", webshopNummer),
                                new GraphQLQueryArgument("RFidNummer", RFidNummer),
                                new GraphQLQueryArgument("qrKode", qrKode),
                                new GraphQLQueryArgument("salgspris", salgspris)
                            });
            var response = await query.Execute();
            JArray result = response["createVare"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> CreateEkstraAsync(
            string beskrivelse,
            string antal,
            string webshopNummer
            )
        {
            var query = client.CreateQuery(builder =>
                builder.Field("createEkstra",
                    createEkstra =>
                        createEkstra
                            .Argument("beskrivelse", "string", "beskrivelse", true)
                            .Argument("antal", "string", "antal", true)
                            .Argument("webshop", "string", "webshop", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("beskrivelse", beskrivelse),
                                new GraphQLQueryArgument("antal", antal),
                                new GraphQLQueryArgument("webshop", webshopNummer)
                            });
            var response = await query.Execute();
            JArray result = response["createEkstra"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> GetAllVare()
        {
            var query = client.CreateQuery(builder =>
                builder.Field("Vare",
                    GetAllItems =>
                        GetAllItems
                            .Field("varenummer")
                            .Field("beskrivelse")
                            .Field("tilgang")
                            .Field("afgang")
                            .Field("ampere")
                            .Field("note")
                            .Field("status")
                            .Field("antal")
                            .Field("vareLokation")
                            .Field("pinNr")
                            .Field("webshopNummer")
                            .Field("length")
                            .Field("rfidNummer")
                            .Field("qrKode")
                            .Field("salgspris")
                            ),
                            APIIP);
            var response = await query.Execute();
            JArray VareResult = response["Vare"];
            List<object> vareList = new List<object>();
            foreach (var item in VareResult)
            {
                vareList.Add(item);
            }
            return VareResult;
        }

        public async Task<JArray> GetEkstraAsync()
        {
            var query = client.CreateQuery(builder =>
                builder.Field("Ekstra",
                    GetAllItems =>
                        GetAllItems
                            .Field("id")
                            .Field("beskrivelse")
                            .Field("antal")
                            .Field("webshop")
                            ),
                            APIIP);
            var response = await query.Execute();
            JArray EkstraResult = response["Ekstra"];
            return EkstraResult;
        }

        public async Task<JArray> GetEventEkstraAsync()
        {
            var query = client.CreateQuery(builder =>
                builder.Field("getEventEkstraTilbehor",
                    GetAllItems =>
                        GetAllItems
                            .Field("Id")
                            .Field("Beskrivelse")
                            .Field("Antal")
                            .Field("EventId")
                            .Field("EventLokation")
                            .Field("Status")
                            ),
                            APIIP);
            var response = await query.Execute();
            JArray EventEkstraResult = response["getEventEkstraTilbehor"];
            return EventEkstraResult;
        }

        public async Task<JArray> EditVareAsync(
            string varenummer,
            string newVarenummer,
            string beskrivelse,
            string tilgang,
            string afgang,
            string ampere,
            string status,
            int antal,
            string varelokation,
            string pinNr,
            int lengde,
            string note,
            string webshopNummer,
            string RFidNummer,
            string qrKode,
            string salsgpris)
        {
            var query = client.CreateQuery(builder =>
                builder.Field("editVare",
                    EditVareAsync =>
                        EditVareAsync
                            .Argument("varenummer", "string", "varenummer", true)
                            .Argument("newVarenummer", "string", "newVarenummer", true)
                            .Argument("beskrivelse", "string", "beskrivelse", true)
                            .Argument("tilgang", "string", "tilgang", true)
                            .Argument("afgang", "string", "afgang", true)
                            .Argument("ampere", "string", "ampere", true)
                            .Argument("status", "string", "status", true)
                            .Argument("antal", "int", "antal", true)
                            .Argument("varelokation", "string", "varelokation", true)
                            .Argument("pinNr", "string", "pinNr", true)
                            .Argument("lengde", "int", "lengde", true)
                            .Argument("note", "string", "note", true)
                            .Argument("webshopNummer", "string", "webshopNummer", true)
                            .Argument("RFidNummer", "string", "RFidNummer", true)
                            .Argument("qrKode", "string", "qrKode", true)
                            .Argument("salgspris", "string", "salgspris", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("varenummer", varenummer),
                                new GraphQLQueryArgument("newVarenummer", newVarenummer),
                                new GraphQLQueryArgument("beskrivelse", beskrivelse),
                                new GraphQLQueryArgument("tilgang", tilgang),
                                new GraphQLQueryArgument("afgang",  afgang),
                                new GraphQLQueryArgument("ampere", ampere),
                                new GraphQLQueryArgument("status", status),
                                new GraphQLQueryArgument("antal", antal),
                                new GraphQLQueryArgument("varelokation", varelokation),
                                new GraphQLQueryArgument("pinNr", pinNr),
                                new GraphQLQueryArgument("lengde", lengde),
                                new GraphQLQueryArgument("note", note),
                                new GraphQLQueryArgument("webshopNummer", webshopNummer),
                                new GraphQLQueryArgument("RFidNummer", RFidNummer),
                                new GraphQLQueryArgument("qrKode", qrKode),
                                new GraphQLQueryArgument("salgspris", salsgpris)
                            });
            var response = await query.Execute();
            JArray result = response["editVare"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }
        public async Task<JArray> EditEkstraAsync(
            string id,
            string beskrivelse,
            string antal,
            string webshopNummer)
        {
            var query = client.CreateQuery(builder =>
                builder.Field("editEkstra",
                    CreateVare =>
                        CreateVare
                            .Argument("id", "string", "id", true)
                            .Argument("beskrivelse", "string", "beskrivelse", true)
                            .Argument("antal", "string", "antal", true)
                            .Argument("webshop", "string", "webshop", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("id", id),
                                new GraphQLQueryArgument("beskrivelse", beskrivelse),
                                new GraphQLQueryArgument("antal", antal),
                                new GraphQLQueryArgument("webshop", webshopNummer)
                            });
            var response = await query.Execute();
            JArray result = response["editEkstra"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> udlejEkstraAsync(
            string webshopNr,
            string eventId,
            string eventLokation,
            string status,
            int antal)
        {
            var query = client.CreateQuery(builder =>
                builder.Field("udlejEkstra",
                    CreateVare =>
                        CreateVare
                            .Argument("webshopNr", "string", "webshopNr", true)
                            .Argument("eventId", "string", "eventId", true)
                            .Argument("eventLokation", "string", "eventLokation", true)
                            .Argument("status", "string", "status", true)
                            .Argument("antal", "int", "antal", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("webshopNr", webshopNr),
                                new GraphQLQueryArgument("eventId", eventId),
                                new GraphQLQueryArgument("eventLokation", eventLokation),
                                new GraphQLQueryArgument("status", status),
                                new GraphQLQueryArgument("antal", antal)
                            });
            var response = await query.Execute();
            JArray result = response["udlejEkstra"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }
        public async Task<JArray> CopyEventAsync(int eventId, string name, string copy)
        {
            var query = client.CreateQuery(builder =>
                builder.Field("copyEvent",
                   CopyEvent =>
                        CopyEvent
                            .Argument("eventId", "int", "eventId", true)
                            .Argument("newEventName", "string", "newEventName", true)
                            .Argument("copyLokation", "string", "copyLokation", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("eventId", eventId),
                                new GraphQLQueryArgument("newEventName", name),
                                new GraphQLQueryArgument("copyLokation", copy)
                            });
            var response = await query.Execute();
            JArray result = response["copyEvent"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> EditEventVareStatusAsync(
            string id,
            string status
            )
        {
            var query = client.CreateQuery(builder =>
                builder.Field("editEventVareStatus",
                    EditEventVare =>
                        EditEventVare
                            .Argument("id", "string", "id", true)
                            .Argument("status", "string", "status", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("id", id),
                                new GraphQLQueryArgument("status", status)
                            });
            var response = await query.Execute();
            JArray result = response["editEventVareStatus"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }
        
public async Task<JArray> EditEventVareLokationAsync(
            string eventId,
            string vareId, 
            string eventLokation
            )
        {
            var query = client.CreateQuery(builder =>
                builder.Field("editEventVareStatusAndLokation",
                    EditEventVareLokation =>
                        EditEventVareLokation
                            .Argument("eventId", "string", "eventId", true)
                            .Argument("vareId", "string", "vareId", true)
                            .Argument("eventLokation", "string", "eventLokation", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("eventId", eventId),
                                new GraphQLQueryArgument("vareId", vareId),
                                new GraphQLQueryArgument("eventLokation", eventLokation)
                            });
            var response = await query.Execute();
            JArray result = response["editEventVareStatusAndLokation"];
            return result;
            // builderResponse["verifiedLogin"]["verified"] ? "true" : "false";
        }

        public async Task<JArray> CreateEventAsync(
            string eventName,
            string startDate,
            string slutDate,
            string note,
            string dieselStart,
            string dieselSlut,
            string defektePærer,
            string tlfNr,
            string aktiv,
            string brugerId
            )
        {
            var query = client.CreateQuery(builder =>
                builder.Field("createEvent",
                    EditEventVare =>
                        EditEventVare
                            .Argument("navn", "string", "navn", true)
                            .Argument("startdato", "string", "startdato", true)
                            .Argument("slutdato", "string", "slutdato", true)
                            .Argument("note", "string", "note", true)
                            .Argument("dieselstanderstart", "string", "dieselstanderstart", true)
                            .Argument("dieselstanderslut", "string", "dieselstanderslut", true)
                            .Argument("defektpere", "string", "defektpere", true)
                            .Argument("tlfnummer", "string", "tlfnummer", true)
                            .Argument("aktiv", "string", "aktiv", true)
                            .Argument("brugerId", "string", "brugerId", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("navn", eventName),
                                new GraphQLQueryArgument("startdato", startDate),
                                new GraphQLQueryArgument("slutdato", slutDate),
                                new GraphQLQueryArgument("note", note),
                                new GraphQLQueryArgument("dieselstanderstart", dieselStart),
                                new GraphQLQueryArgument("dieselstanderslut", dieselSlut),
                                new GraphQLQueryArgument("defektpere", defektePærer),
                                new GraphQLQueryArgument("tlfnummer", tlfNr),
                                new GraphQLQueryArgument("aktiv", aktiv),
                                new GraphQLQueryArgument("brugerId", brugerId)
                            });
            var response = await query.Execute();
            JArray result = response["createEvent"];
            return result;
            
        }

        public async Task<JArray> CreateEventLokationAsync(
            int eventId,
            string lokation
            )
        {
            var query = client.CreateQuery(builder =>
                builder.Field("createEventLokation",
                    EditEventVare =>
                        EditEventVare
                            .Argument("eventId", "int", "eventId", true)
                            .Argument("lokation", "string", "lokation", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("eventId", eventId),
                                new GraphQLQueryArgument("lokation", lokation)
                            });
            var response = await query.Execute();
            JArray result = response["createEventLokation"];
            return result;

        }

        public async Task<JArray> GetEventLokation(int eventId)
        {
            var query = client.CreateQuery(builder =>
                builder.Field("EventLokation",
                    EventLokation =>
                        EventLokation
                            .Argument("eventId", "int", "eventId", true)
                            .Field("EventLokationId")
                            .Field("EventLokationEventId")
                            .Field("Lokation")
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("eventId", eventId)
                            });
            var response = await query.Execute();
            JArray result = response["EventLokation"];
            return result;

        }



        public async Task<JArray> EditEvent(
            string navn,
            string newnavn,
            string startdato,
            string slutdato,
            string note,
            string dieselstanderstart,
            string dieselstanderslut,
            string defektpere,
            string tlfnummer,
            string aktiv,
            string brugerId
            )
        {
            var query = client.CreateQuery(builder =>
                builder.Field("editEvent",
                    editEvent =>
                        editEvent
                            .Argument("navn", "string", "navn", true)
                            .Argument("newnavn", "string", "newnavn", true)
                            .Argument("startdato", "string", "startdato", true)
                            .Argument("slutdato", "string", "slutdato", true)
                            .Argument("note", "string", "note", true)
                            .Argument("dieselstanderstart", "string", "dieselstanderstart", true)
                            .Argument("dieselstanderslut", "string", "dieselstanderslut", true)
                            .Argument("defektpere", "string", "defektpere", true)
                            .Argument("tlfnummer", "string", "tlfnummer", true)
                            .Argument("aktiv", "string", "aktiv", true)
                            .Argument("brugerId", "string", "brugerId", true)
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("navn", navn),
                                new GraphQLQueryArgument("newnavn", newnavn),
                                new GraphQLQueryArgument("startdato", startdato),
                                new GraphQLQueryArgument("slutdato", slutdato),
                                new GraphQLQueryArgument("note", note),
                                new GraphQLQueryArgument("dieselstanderstart", dieselstanderstart),
                                new GraphQLQueryArgument("dieselstanderslut", dieselstanderslut),
                                new GraphQLQueryArgument("defektpere", defektpere),
                                new GraphQLQueryArgument("tlfnummer", tlfnummer),
                                new GraphQLQueryArgument("aktiv", aktiv),
                                new GraphQLQueryArgument("brugerId", brugerId)
                            });
            var response = await query.Execute();
            JArray result = response["editEvent"];
            return result;

        }


        public async Task<JArray> editEventLocation(
            string id,
            string eventid,
            string lokation

         )
        {
            var query = client.CreateQuery(builder =>
                builder.Field("editEventLocation",
                    editEvent =>
                        editEvent
                            .Argument("id", "string", "id", true)
                            .Argument("eventId", "string", "eventId", true)
                            .Argument("lokation", "string", "lokation", true)
                            
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("id", id),
                                new GraphQLQueryArgument("eventId", eventid),
                                new GraphQLQueryArgument("lokation", lokation),
                               
                            });
            var response = await query.Execute();
            JArray result = response["editEventLocation"];
            return result;

        }

        public async Task<JArray> EventVareHistorik(string vareId)
        {
            var query = client.CreateQuery(builder =>
                builder.Field("RecentEvents",
                    recentEvents =>
                        recentEvents
                            .Argument("vareId", "int", "vareId")
                            .Field("eventId")
                            .Field("navn")
                            ),
                            APIIP, arguments: new[] {
                                new GraphQLQueryArgument("vareId", vareId),
                            });
            var response = await query.Execute();
            JArray result = response["RecentEvents"];
            return result;
        }
        
        public async Task<JArray> GetLastVare()
        {
            var query = client.CreateQuery(builder =>
                    builder.Field("GetLastVare",
                        getLastVare =>
                            getLastVare
                                .Field("varenummer")
                                .Field("beskrivelse")
                                .Field("tilgang")
                                .Field("afgang")
                                .Field("ampere")
                                .Field("note")
                                .Field("status")
                                .Field("antal")
                                .Field("vareLokation")
                                .Field("pinNr")
                                .Field("webshopNummer")
                                .Field("length")
                                .Field("rfidNummer")
                                .Field("qrKode")
                                .Field("salgspris")
                                .Field("modified")
                    ),
                APIIP
            );
            var response = await query.Execute();
            JArray result = response["GetLastVare"];
            return result;
        }
        
public async Task<JArray> GetEventVare(string EventId, string status)
        {
            var query = client.CreateQuery(builder =>
                    builder.Field("eventVare",
                        GetEventVare =>
                            GetEventVare
                                .Argument("eventId", "string", "eventId")
                                .Argument("status", "string", "status")
                                .Field("Id")
                                .Field("EventId")
                                .Field("VareId")
                                .Field("EventLokation")
                                .Field("Aflevere")
                                .Field("WebshopVarenummer")
                                .Field("StartDato")
                                .Field("SlutDato")
                                .Field("Status")
                                .Field("Beskrivelse")
                                .Field("EventName")
                                .Field("Ampere")
                                .Field("PinNr")
                                .Field("Container")
                    ),
                    APIIP, arguments: new[] {
                        new GraphQLQueryArgument("eventId", EventId),
                        new GraphQLQueryArgument("status", status),
                    });
            var response = await query.Execute();
            JArray result = response["eventVare"];
            return result;
        }

        public async Task<JArray> CreateEventVareVareId(string EventId, string vareId, string eventLokation, string status)
        {
            var query = client.CreateQuery(builder =>
                    builder.Field("createEventVareRFID",
                        createEventVareRFID =>
                            createEventVareRFID
                                .Argument("eventId", "string", "eventId")
                                .Argument("vareId", "string", "vareId")
                                .Argument("eventLokation", "string", "eventLokation")
                                .Argument("status", "string", "status")
                    ),
                APIIP, arguments: new[] {
                    new GraphQLQueryArgument("eventId", EventId),
                    new GraphQLQueryArgument("vareId", vareId),
                    new GraphQLQueryArgument("eventLokation", eventLokation),
                    new GraphQLQueryArgument("status", status),
                });
            var response = await query.Execute();
            JArray result = response["createEventVareRFID"];
            return result;
        }


        public async Task<JObject> CreateEventVareRfid(string EventId, string rfidNr, string eventLokation, string status)
        {
            var query = client.CreateQuery(builder =>
                    builder.Field("createEventVareRFID",
                        createEventVareRFID =>
                            createEventVareRFID
                                .Argument("eventId", "string", "eventId")
                                .Argument("rfidNr", "string", "rfidNr")
                                .Argument("eventLokation", "string", "eventLokation")
                                .Argument("status", "string", "status")
                    ),
                APIIP, arguments: new[] {
                    new GraphQLQueryArgument("eventId", EventId),
                    new GraphQLQueryArgument("rfidNr", rfidNr),
                    new GraphQLQueryArgument("eventLokation", eventLokation),
                    new GraphQLQueryArgument("status", status),
                });
            var response = await query.Execute();
            JObject result = response["createEventVareRFID"];
            return result;
        }

        public async Task<JObject> AflevereEventVareRfid(string EventId, string rfidNr, string eventLokation, string status, string bruger)
        {
            var query = client.CreateQuery(builder =>
                    builder.Field("AflevereEventVareRFID",
                        createEventVareRFID =>
                            createEventVareRFID
                                .Argument("eventId", "string", "eventId")
                                .Argument("rfidNr", "string", "rfidNr")
                                .Argument("aflevere", "string", "aflevere")
                                .Argument("eventLokation", "string", "eventLokation")
                                .Argument("status", "string", "status")
                    ),
                APIIP, arguments: new[] {
                    new GraphQLQueryArgument("eventId", EventId),
                    new GraphQLQueryArgument("rfidNr", rfidNr),
                    new GraphQLQueryArgument("aflevere", bruger),
                    new GraphQLQueryArgument("eventLokation", eventLokation),
                    new GraphQLQueryArgument("status", status),
                });
            var response = await query.Execute();
            JObject result = response["AflevereEventVareRFID"];
            return result;
        }

        public async Task<JArray> tilføjRfidTilVare(string VareNr, string Rfid)
        {
            var query = client.CreateQuery(builder =>
                    builder.Field("editVareRfid",
                        editVareRfid =>
                            editVareRfid
                                .Argument("varenummer", "string", "varenummer")
                                .Argument("rfid", "string", "rfid")
                    ),
                APIIP, arguments: new[] {
                    new GraphQLQueryArgument("varenummer", VareNr),
                    new GraphQLQueryArgument("rfid", Rfid),
                    
                });
            var response = await query.Execute();
            JArray result = response["editVareRfid"];
            return result;
        }
        
public async Task<JArray> editVarewebshopNummer(string VareNr, string WebshopNummer)
        {
            var query = client.CreateQuery(builder =>
                    builder.Field("editVarewebshopNummer",
                        editVareRfid =>
                            editVareRfid
                                .Argument("varenummer", "string", "varenummer")
                                .Argument("WebshopNummer", "string", "WebshopNummer")
                    ),
                APIIP, arguments: new[] {
                    new GraphQLQueryArgument("varenummer", VareNr),
                    new GraphQLQueryArgument("WebshopNummer", WebshopNummer),

                });
            var response = await query.Execute();
            JArray result = response["editVarewebshopNummer"];
            return result;
        }

    }
}