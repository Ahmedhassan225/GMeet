import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[] | null;;
  pagination: Pagination;
  container: 'Unread';
  pageNumer = 1;
  pageSize = 5;
  loadingFlag = false;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
    this.LoadMessages();
  }

  LoadMessages(){
    this.loadingFlag = true;
    this.messageService.getMessages(this.pageNumer, this.pageSize, this.container).subscribe(res =>{
      this.messages = res.result;
      this.pagination = res.pagination;
      this.loadingFlag = false;
      console.log(this.messages);
    })
  }

  pageChanged(event: any){
    this.pageNumer = event.page;
    this.LoadMessages();
  }

  deleteMessage(id :number){
    this.messageService.deleteMessage(id).subscribe(() =>{
      this.messages?.splice(this.messages.findIndex(m => m.id === id), 1);
    })
  }

}
