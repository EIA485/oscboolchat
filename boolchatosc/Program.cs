using boolchatosc;
using System.Net;
Console.WriteLine("program started");

//define constants and instances

SimpleOSC.OSCMessage t = new SimpleOSC.OSCMessage { path = "/avatar/parameters/boolchat", arguments = new object[1] { 1 }, typeTag = "" };
SimpleOSC.OSCMessage f = new SimpleOSC.OSCMessage { path = "/avatar/parameters/boolchat", arguments = new object[1] { 0 }, typeTag = "" };

statemngr oscstate = new("osc");
statemngr wsstate = new("WebSocket");

wsmngr ws = new(new("wss://logix.newweb.page/boolchat"));

SimpleOSC OSC = new();
byte[] oscBuf = new byte[65535];
List<SimpleOSC.OSCMessage> msg = new List<SimpleOSC.OSCMessage>();

Console.WriteLine("connecting to osc");

OSC.OpenClient(9001);
OSC.SetUnconnectedEndpoint(new IPEndPoint(IPAddress.Loopback, 9000));




while (true)
{
    OSC.GetIncomingOSC(msg);
    foreach (SimpleOSC.OSCMessage m in msg)
    {
        if (m.path == "/avatar/parameters/boolchat" && m.arguments.Length == 1 && m.arguments[0].GetType() == typeof(bool))
            oscstate.cur = (bool)m.arguments[0];
    }
    msg.Clear();

    bool? wsIn = ws.receive();
    if (wsIn.HasValue)
        wsstate.cur = wsIn.Value;

    if (oscstate.cur != wsstate.cur)
    {
        Console.WriteLine("updating other");
        if (oscstate.changed)
        {
            ws.send(oscstate.cur);
            wsstate.cur = (oscstate.cur);
        }
        else if (wsstate.changed)
        {
            OSC.SendOSCPacket(wsstate.cur ? t : f, oscBuf);
            oscstate.cur = (wsstate.cur);
        }
    }
    oscstate.changed = false;
    wsstate.changed = false;
    Thread.Sleep(100);
}

