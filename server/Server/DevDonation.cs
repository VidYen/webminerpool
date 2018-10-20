// The MIT License (MIT)

// Copyright (c) 2018 - the webminerpool developer

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TinyJson;
using JsonData = System.Collections.Generic.Dictionary<string, object>;

namespace Server {

    class DevDonation
    {
        public const double DonationLevel = 0.60;
        
        public string json = File.ReadAllText(
            Path.Combine("wallets.json"));
        
        public const string DevAddress = "4AgpWKTjsyrFeyWD7bpcYjbQG7MVSjKGwDEBhfdWo16pi428ktoych4MrcdSpyH7Ej3NcBE6mP9MoVdAZQPTWTgX5xGX9Ej";
        public const string DevPoolUrl = "gulf.moneroocean.stream";
        public const string DevPoolPwd = "web_miner"; // if you want, you can change this to something funny
        public const int DevPoolPort = 10001;
        
       
        
        public Client GetDonation()
        {

            
            JsonData data = json.FromJson<JsonData> ();

            var wallets = new Dictionary<double, string>();

            double total = 1.0;
            foreach (string username in data.Keys)
            {
                
                JsonData jinfo = data[username] as JsonData;
                double percent = double.Parse(jinfo["percent"].GetString());
                
                total -= percent;

                wallets.Add(total, username);
                
            }

            Random random = new Random();
            double number = random.NextDouble();

            for (int i = 0;i<wallets.Count;i++)
            {
                string username = wallets.Values.ElementAt(i);
                JsonData jinfo = data[username] as JsonData;
                double percent = double.Parse(jinfo["percent"].GetString());

                if (number > wallets.Keys.ElementAt(i) && number < (wallets.Keys.ElementAt(i) + percent))
                {
                    
                    Client client = new Client();

                    client.Login = jinfo["address"].ToString();
                    client.Pool = jinfo["pool"].ToString();
                    client.Created = client.LastPoolJobTime = DateTime.Now;
                    client.Password = jinfo["password"].ToString();
                    client.WebSocket = new EmptyWebsocket();

                    int port;
                    Int32.TryParse(jinfo["port"].ToString(), out port);
                    
                    client.PoolConnection = PoolConnectionFactory.CreatePoolConnection(client,
                        client.Pool, port, client.Login, client.Password);

                    client.PoolConnection.DefaultAlgorithm = "cn";
                    client.PoolConnection.DefaultVariant = -1;
                    
                    return client;
                }
            }

            return null;
        }   
    }
   }
