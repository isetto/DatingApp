import { Component, OnInit } from '@angular/core';
import { Message } from '../models/message';
import { Pagination } from '../models/pagination';
import { MessageService } from '../services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit {
  messages: Message[] = []
  pagination: Pagination
  container = 'Unread'
  pageNumber = 1
  pageSize = 5
  loadingFlag = false
  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
    this.loadMessages()
  }

  loadMessages(){
    this.loadingFlag = true
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(response=>{
      this.messages = response.result
      this.pagination = response.pagination
      this.loadingFlag = false

    })
  }

  deleteMessage(messageId: number){
    this.messageService.deleteMessage(messageId).subscribe(()=>{
      this.messages.splice(this.messages.findIndex((message: Message)=> message.id === messageId), 1)
    })
  }

  pageChanged(event: any){
    this.pageNumber = event.page
    this.loadMessages()
  }

}
