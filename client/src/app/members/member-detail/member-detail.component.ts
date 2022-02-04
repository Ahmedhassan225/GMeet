import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit ,OnDestroy{
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  member: Member
  gallaryOptions: NgxGalleryOptions[];
  gallaryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] = [];
  user :User;

  constructor(private memberService: MembersService, private route: ActivatedRoute
    , private messageService: MessageService, private accountService :AccountService, private router: Router) {

    this.accountService.curentUserSource$.pipe(take(1)).subscribe(user => this.user = user as User);
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit(): void {
    
    this.route.data.subscribe(data =>{
      this.member = data['member'];
    })
    this.route.queryParams.subscribe(params => {
      params['tab'] ? this.selectTab(params['tab']) : this.selectTab(0);
    })

    this.gallaryOptions = [{
      width: '500px',
      height: '500px',
      imagePercent: 100,
      thumbnailsColumns: 4,
      imageAnimation: NgxGalleryAnimation.Slide,
      preview:false
    }]

    this.gallaryImages = this.getImages();
  }

  getImages(): NgxGalleryImage[]{
    const imagesUrls = [] as  any;
    
    this.member.photos.forEach(photo => {
      imagesUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url,
      })
    });

    return imagesUrls;
  }

  loadMessages(){
    this.messageService.getMessageThread(this.member.userName).subscribe(res =>{
      this.messages = res;
    });
  }

  onTabActivated(data :TabDirective){
    this.activeTab = data;
    if(this.activeTab.heading == 'Messeges' && this.messages.length === 0){
      this.messageService.createHubConnection(this.user, this.member.userName);
    }
    else{
      this.messageService.stopHubConnection();
    }
  }

  selectTab(tabId: number){
    this.memberTabs.tabs[tabId].active = true;
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
