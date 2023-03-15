// See https://aka.ms/new-console-template for more information

using ApnsSender;

Console.WriteLine("Hello, World!");

var bundleId = "";
var teamId = "";
var keyId = "";
var deviceToken = "";
var p8PrivateKey = File.ReadAllText("cert.p8");

await Apn.SendTestNotificationAsync(deviceToken,p8PrivateKey, teamId, keyId, bundleId);

Console.ReadLine();
