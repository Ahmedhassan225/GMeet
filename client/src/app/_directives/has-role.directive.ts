import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit{
  @Input() appHasRole: string[]
  CurrentUser: User;

  constructor(private viewContainerRef: ViewContainerRef,
     private templateRaf: TemplateRef<any>, private accountService: AccountService) {
       this.accountService.curentUserSource$.pipe(take(1)).subscribe(user =>{
          if(user) 
            this.CurrentUser = user;
       })
      }
  
      ngOnInit(){
        //clear view if no roles
        if(!this.CurrentUser?.roles || this.CurrentUser == null){
          this.viewContainerRef.clear();
          return;
        }

        if(this.CurrentUser?.roles.some(r => this.appHasRole.includes(r))){
          this.viewContainerRef.createEmbeddedView(this.templateRaf);
        }
        else{
          this.viewContainerRef.clear();
        }
      }
}
