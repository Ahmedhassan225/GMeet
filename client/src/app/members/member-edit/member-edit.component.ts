import { Component, OnInit,ViewChild, HostListener } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { take } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { NgForm } from '@angular/forms';
import { PreventUnsavedChangesGuard } from 'src/app/_guards/prevent-unsaved-changes.guard';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  formEdited :boolean = false;
  member: Member;
  user: User |null = null;
  @ViewChild('editForm') editForm: NgForm;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event:any){
    if(this.editForm.dirty){
      $event.returnValue = true;
    }
  }
  constructor(private accountService: AccountService, private memberService: MembersService,private toastr: ToastrService) {
    this.accountService.curentUserSource$.pipe(take(1)).subscribe(user => this.user = user);
   }

  ngOnInit(): void {
    this.loadMember();
    
  }

  loadMember(){
    if(this.user){
      this.memberService.getMember(this.user.username).subscribe(member => {
        this.member = member;
      })
    }
  }

  updateMember(){
    this.memberService.updateMember(this.member).subscribe(member =>{
      this.toastr.success('Updated Successfully')
      this.formEdited = true;
      this.editForm.reset(this.member);
    })
  }
}
