export class RoomMessage {  
   
    message: string;  
    room: string;  
    
    constructor(message: string='',room:string='') {  
      this.message = message;  
      this.room = room;  
    }  
  }  