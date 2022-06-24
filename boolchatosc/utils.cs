using System.Net.WebSockets;

namespace boolchatosc
{
    class statemngr
    {
        private bool currentState = false;

        private string name;
        static private statemngr[] instances = new statemngr[0];

        public statemngr(string name) => this.name = name;

        public bool cur
        {
            set
            {
                currentState = value;
                changed = true;
                Console.WriteLine($"{name} set state to {currentState}");
            }
            get { return currentState; }
        }

        public bool changed = false;
    }

    class wsmngr
    {
        private ClientWebSocket ws = new();
        private ArraySegment<byte> inBuf = new(new byte[10]);
        Task<WebSocketReceiveResult> wsRecive;
        static readonly ArraySegment<byte> t = new(new byte[] { 116, 114, 117, 101 }); //true
        static readonly ArraySegment<byte> f = new(new byte[] { 102, 097, 108, 115, 101 }); //false

        public wsmngr(Uri uri)
        {
            ws.ConnectAsync(uri, new());
        }

        public void send(bool state)
        {
            ws.SendAsync(state ? t : f, WebSocketMessageType.Text, true, new());
        }

        public bool? receive()
        {
            if (wsRecive == null)
            {
                if (ws.State == WebSocketState.Open)
                    wsRecive = ws.ReceiveAsync(inBuf, new());
                else
                    return null;
            }

            if (wsRecive.IsCompleted)
            {
                wsRecive = null;
                return inBuf[0] == 116;//t
            }
            return null;
        }
    }
}
