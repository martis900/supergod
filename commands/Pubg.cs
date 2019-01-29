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
using System.Threading;

namespace SUPERGOD {
    public class Pubg : ModuleBase<SocketCommandContext> {
        [Command("last")]
        public async Task PubgAsync(params string[] names) {

            string token = "YOUR TOKEN GOES HERE!!!!!!";

            string PlayerURL = "https://api.playbattlegrounds.com/shards/pc-eu/players?filter[playerNames]="; // URL TO GET PLAYERS DATA

            string MatchURL = "https://api.playbattlegrounds.com/shards/pc-eu/matches/"; // URL TO GET MATCH INFO BY ID

            if (names.Length > 4) {
                await ReplyAsync("names" + ">4");
            }

            //GET PLAYER JSON FILE AND GET LAST MATCH ID

            for (int i = 0; i < names.Length; i++) {
                if (names.Length > 0 && names.Length < 3) {
                    PlayerURL += names[i].ToString();
                    if (i + 1 != names.Length) {
                        PlayerURL += ",";
                    }
                }
                else {
                    break;
                }

            }

            await ReplyAsync("", false, table(names, token, MatchURL, PlayerURL).Build());


        }
        public EmbedBuilder table(string[] names, string token, string MatchURL, string PlayerURL) {
            var builder = new EmbedBuilder();
            builder.WithTitle("🕹️ LAST PUBG MATCH STATS 🎮")
                .WithDescription("––––––––––––––––––");

            var PlayerFile = GetJsonFileFromPubgApi(PlayerURL, token);

            JObject playerObject = JObject.Parse(PlayerFile);

            for (int i = 0; i < names.Length; i++) {
                var lastMatchID = playerObject["data"][i]["relationships"]["matches"]["data"][0]["id"].ToString();
                // !!!!!!!!!!!!!!!! NEED TO CHECK IF MATCH ID IS EQUAL IF THEY HAVE PLAYED SAME MATCH
                //WITH LAST MATH ID FIND PULL INFO ABOUT PLAYER IN MATCH
                var MatchFile = GetJsonFileFromPubgApi(MatchURL + lastMatchID, token);

                JObject matchObject = JObject.Parse(MatchFile);

                // GET MAP STATS
                var map = matchObject["data"]["attributes"]["mapName"].ToString();
                var duration = matchObject["data"]["attributes"]["duration"].ToString();

                if(playerIndex(JArray.Parse(matchObject["included"].ToString()), names[i]) == -1) {
                    Console.WriteLine("ERRRRRRROR");
                    break;
                }

                // GETTING ARRAY THERE EVERY PARTISIPENT IS SAVED


                
                // GETTING STAT BY ITS NAME
                var playerArray = JObject.Parse(matchObject["included"][playerIndex(JArray.Parse(matchObject["included"].ToString()), names[i])].ToString());

                //CREATING MESSAGE FORMAT AND CONTENT
                builder.AddInlineField(names[i].ToString(),
                "🗺️: " + map.Replace("_Main", "") + "\n" + 
                "⏰: " + (Int32.Parse(duration) / 60).ToString() + "min" + "\n" +
                "🏆: " + GetPlayersStat("winPlace", playerArray) + "th place" + "\n" +
                "🕰️: " + (Int32.Parse(GetPlayersStat("timeSurvived", playerArray)) / 60).ToString() + "min survived" + "\n" +
                "🔫: " + GetPlayersStat("kills", playerArray) + " kills" + "\n" +
                "🤕: " + GetPlayersStat("headshotKills", playerArray) + " headshots" + "\n" +
                "🚶: " + GetPlayersStat("walkDistance", playerArray) + " meters" + "\n" +
                "🚗: " + GetPlayersStat("rideDistance", playerArray) + " meters" + "\n" + 
                "🔎 : " + GetPlayersStat("longestKill", playerArray) + " meters")
                .Color = Color.Green;
                builder.ThumbnailUrl = "https://orig00.deviantart.net/4c4f/f/2017/176/f/d/playerunknown_s_battlegrounds_round_icon_by_eclipx-dbe0jrh.png";

            }

            return builder;

        }

        // PULL FILE FROM PUBG API AND CONVERT IT TO JSON STRING
        public string GetJsonFileFromPubgApi(string url, string token){
            try{
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = WebRequestMethods.Http.Get;
                req.Headers.Add("Authorization", token);
                req.Headers.Add("Accept", "application/vnd.api+json");

                var response = (HttpWebResponse)req.GetResponse();

                using (var sr = new StreamReader(response.GetResponseStream())){
                    var jsonText = sr.ReadToEnd();
                    jsonText.Replace(@"\", "");
                    return jsonText;
                }
            }
            catch (Exception e){
                throw e;
            }

        }

        public int playerIndex(JArray matchObject, string name){
            for (int i = 0; i < matchObject.Count; i++){
                if (matchObject[i]["type"].ToString() == "participant"){
                    if (matchObject[i]["attributes"]["stats"]["name"].ToString() == name)
                        return i;
                    else
                        continue;
                    
                }
                else {
                    continue;
                }
            }
            return -1;
        }

        public string GetPlayersStat(string name, JObject matchObject) {
            var a = matchObject["attributes"]["stats"][name].ToString();
            return a;
        }
    }
}


