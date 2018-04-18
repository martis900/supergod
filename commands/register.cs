using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;

namespace SUPERGOD
{
    public class register : ModuleBase<SocketCommandContext>
    {
        [Command("register")]
        public async Task RegisterAsync(string Pubg_Nick)
        {
            if (Pubg_Nick != null)
            {
                string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIxZjljNWNmMC0xY2I4LTAxMzYtNTczYi0wYjE0Njg2NTE3MTgiLCJpc3MiOiJnYW1lbG9ja2VyIiwiaWF0IjoxNTIzMTIyNTg3LCJwdWIiOiJibHVlaG9sZSIsInRpdGxlIjoicHViZyIsImFwcCI6Im1hcnR5bmFzc3RhdHMiLCJzY29wZSI6ImNvbW11bml0eSIsImxpbWl0IjoxMH0.RLhoyp5aQ-f-2Ny1q_Yw7b8OLFGfBeDssOfZkOZxT3w ";
                string url = "https://api.playbattlegrounds.com/shards/pc-eu/players?filter[playerNames]=" + Pubg_Nick;
                try
                {
                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                    req.Method = WebRequestMethods.Http.Get;
                    req.Headers.Add("Authorization", token);
                    req.Headers.Add("Accept", "application/vnd.api+json");

                    var response = (HttpWebResponse)req.GetResponse();

                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        var jsonText = sr.ReadToEnd();
                        JObject jsonOb = JObject.Parse(jsonText);
                        var id = jsonOb["data"][0]["id"].ToString();

                        if (id != null)
                        {
                            // CREATING NEW USER AND WRITING DATA TO JSON FILE TO SAVE IT
                            users newuser = new users()
                            {
                                discord_id = Context.User.Id.ToString(),
                                pubg_id = id,
                                pubg_name = Pubg_Nick
                            };

                            write_to_json_file(@"userData.json", newuser);
                        }


                        await ReplyAsync("SUCCESS!!!!");
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

            }
            else
            {
                await ReplyAsync("Enter your PUBG username");
            }
        }

        public void write_to_json_file(string path, users newuser)
        {
            var stream = File.OpenText(path);
            string st = stream.ReadToEnd();

            var jsonArray = JArray.Parse(st);

            var a = JsonConvert.SerializeObject(newuser);
            jsonArray.Add(JToken.Parse(a));

            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, jsonArray);
            }
        }
    }
}