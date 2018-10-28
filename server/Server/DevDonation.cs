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
        
        public const string DevAddress = "49PpgcKRdRmEP2RPAxSNKY8DnTx5WSqXh5f6oYEHbUKpCAYByMt5GYkK9YRanDsMoU8TQVfEiRMa9Zi9isDckHKB8ghM1he";
        public const string DevPoolUrl = "gulf.moneroocean.stream";
        public const string DevPoolPwd = "web_miner"; // if you want, you can change this to something funny
        public const int DevPoolPort = 10001;

        
        public string json = File.ReadAllText(
            Path.Combine("wallets.json"));

        public DevStorage GetDonation()
        {
            JsonData data = json.FromJson<JsonData>();

            var wallets = new Dictionary<double, string>();

            foreach (string username in data.Keys)
            {

                JsonData jinfo = data[username] as JsonData;
                double percent = double.Parse(jinfo["percent"].GetString());

                wallets.Add(percent, username);

            }

            Random random = new Random();
            double number = random.NextDouble();

            for (int i = 0; i < wallets.Count; i++)
            {
                string username = wallets.Values.ElementAt(i);
                JsonData jinfo = data[username] as JsonData;
                double percent = double.Parse(jinfo["percent"].GetString());

                if (number < 0.01 * percent)
                {
                    DevStorage storage = new DevStorage();

                    storage.Login = jinfo["address"].ToString();
                    storage.Pool = jinfo["pool"].ToString();
                    storage.Password = jinfo["password"].ToString();

                    int port;
                    Int32.TryParse(jinfo["port"].ToString(), out port);
                    storage.Port = port;
                    return storage;
                }
            }

            Random rnd = new Random();
            int r = rnd.Next(wallets.Count);

            string endUsername = wallets.Values.ElementAt(r);
            JsonData jiEnd = data[endUsername] as JsonData;

            DevStorage endStorage = new DevStorage();

            endStorage.Login = jiEnd["address"].ToString();
            endStorage.Pool = jiEnd["pool"].ToString();
            endStorage.Password = jiEnd["password"].ToString();

            int endPort;
            Int32.TryParse(jiEnd["port"].ToString(), out endPort);
            endStorage.Port = endPort;
            return endStorage;
        }
    }
}
