package main

import (
	"flag"
	"fmt"
	"log"
	"net/url"
	"os"
	"os/signal"
	"time"

	"github.com/gorilla/websocket"
)

var (
	host  = flag.String("host", "127.0.0.1", "The server host.")
	path  = flag.String("path", "", "The service path.")
	query = flag.String("query", "", "The query string.")
)

type WSClient struct {
	c *websocket.Conn
}

func NewWSClient() *WSClient {
	ret := new(WSClient)
	return ret
}

func (p *WSClient) Connect(host string, path string, query string) bool {
	u := url.URL{
		Scheme:   "ws",
		Host:     host,
		Path:     path,
		RawQuery: query,
	}

	interrupt := make(chan os.Signal, 1)
	signal.Notify(interrupt, os.Interrupt)

	c, resp, err := websocket.DefaultDialer.Dial(u.String(), nil)
	if err != nil {
		log.Println("Error at dial:", resp, err)
		return false
	}
	defer c.Close()

	done := make(chan struct{})

	go func() {
		defer c.Close()
		defer close(done)
		for {
			_, message, err := c.ReadMessage()
			current := time.Now() // Do it ASAP.
			if err != nil {
				continue
			}

			original, _ := time.Parse("01/02/2006 15:04:05.000000", string(message))

			fmt.Println("Original: [", string(message), "], received at: [", current.UTC().Format("01/02/2006 15:04:05.000000 MST"), "], difference: [", current.UTC().Sub(original), "]")
		}
	}()

	for {
		select {
		case <-interrupt:
			// To cleanly close a connection, a client should send a close frame and wait for the server to close the connection.
			err := c.WriteMessage(websocket.CloseMessage, websocket.FormatCloseMessage(websocket.CloseNormalClosure, ""))
			if err != nil {
				log.Println("Error at close:", err)
				return false
			}
			select {
			case <-done:
			case <-time.After(time.Second):
			}
			c.Close()
			return true
		}
	}
}

func main() {
	flag.Parse()

	ws := NewWSClient()
	ws.Connect(*host, *path, *query)
}
