import { Component, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

@Component({
  selector: 'app-chat-room1',
  templateUrl: './chat-room1.component.html',
  styleUrls: ['./chat-room1.component.scss']
})
export class ChatRoom1Component implements OnInit {

  public hubConnection: HubConnection;

  constructor() { }

  ngOnInit() {
    //this.signalRConnection();
  }

  ngOnDestroy() {
    //this.closeSignalRConnection();
  }

  private closeSignalRConnection() {
    if(this.hubConnection){
      this.hubConnection.onclose(() => { });
      this.hubConnection.stop();
      this.hubConnection.off("SendAll");
    }
  }

  signalRConnection() {
    if (!this.hubConnection) {
      let builder = new HubConnectionBuilder();
      console.log("creating hub connection");
      // as per setup in the startup.cs
      this.hubConnection = builder.withUrl('http://localhost:5000/hubs/echo').build();
      //this.hubConnection = builder.withUrl('http://localhost:5000/hubs/echo').build();
      // message coming from the server
      this.hubConnection.on("SendAll", (message) => {
        //message logic here
      });
    }
    this.hubConnection.onclose(() => {
      this.hubConnection.start();
    })
    // starting the connection
    this.hubConnection.start().catch(() => {
      console.log("Connection to host failed");
    });
  }

}
