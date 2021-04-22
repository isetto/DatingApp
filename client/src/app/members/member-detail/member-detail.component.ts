import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/models/Member';
import { Message } from 'src/app/models/message';
import { Photo } from 'src/app/models/photo';
import { User } from 'src/app/models/User';
import { AccountService } from 'src/app/services/account.service';
import { MembersService } from 'src/app/services/members.service';
import { MessageService } from 'src/app/services/message.service';
import { PresenceService } from 'src/app/services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.scss']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent
  member: Member
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] = []
  user: User
  constructor(public presence: PresenceService, private route: ActivatedRoute, private messageService: MessageService,
    private accountService: AccountService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(user=>this.user = user)
     }


  ngOnInit(): void {
    this.route.data.subscribe(data=>{
      this.member = data.member
    })
    
    this.route.queryParams.subscribe(params=>{
      params.tab ? this.selectTab(params.tab) : this.selectTab(0)
    })
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]
    this.galleryImages = this.getImages()
 
  }

  getImages(): NgxGalleryImage[]{
    const imageUrls = []
    this.member.photos.forEach((photo: Photo)=>{
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url
      })
    })
    return imageUrls
  }

  loadMessages(){
    this.messageService.getMessageThread(this.member.username).subscribe((messages: Message[])=>{
      this.messages = messages
    })
  }

  selectTab(tabId: number){
    this.memberTabs.tabs[tabId].active=true
  }

  onTabActivated(data: TabDirective){
    this.activeTab = data
    if(this.activeTab.heading == 'Messages' && this.messages.length === 0){
      this.messageService.createHubConnection(this.user, this.member.username)
    }else{
      this.messageService.stopHubConnection()
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection()
  }

}
