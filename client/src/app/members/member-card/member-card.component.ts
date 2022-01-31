import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member
  constructor(private memberservice: MembersService, private toastr :ToastrService, public presence :PresenceService) { 
  }

  ngOnInit(): void {
    console.log(this.presence.onlineUsers$);
    
  }

  addLike(member: Member){
    this.memberservice.addLike(member.userName).subscribe(() => {
      this.toastr.success('You have Liked ' + member.userName);
    })
  }

}
