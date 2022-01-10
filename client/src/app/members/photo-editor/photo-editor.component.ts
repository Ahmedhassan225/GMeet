import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member;
  uploader: FileUploader;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
  user: User |null = null;
  constructor(private accountService: AccountService, private memberService: MembersService) {
    this.accountService.curentUserSource$.pipe(take(1)).subscribe(user => this.user = user);
   }

  ngOnInit(): void {
    this.initializeUploader();
  }

  updateMainphoto(photo: Photo){
    this.memberService.updateMainphoto(photo.id).subscribe(() =>{
      if(this.user != null){
        this.user.photoUrl = photo.url;
        this.accountService.setCurrentUser(this.user as any);
        this.member.photoUrl = photo.url;
        this.member.photos.forEach(p =>{
          if(p.isMain) p.isMain = false;
          if(p.id == photo.id) p.isMain = true;
        });
      }
    });

  }

  deletephoto(photo: Photo){
    this.memberService.deletePhoto(photo.id).subscribe(() =>{
      if(this.user != null){
        this.member.photos.filter(x => x.id !== photo.id);
      }}
    );

  }

  fileOverBase(e: any){
    this.hasBaseDropzoneOver = e;
  }

  initializeUploader(){
    if(this.user != null){
      this.uploader = new FileUploader(
        {
        url: this.baseUrl + 'users/add-photo',
        authToken: 'Bearer ' + this.user.token,
        isHTML5: true,
        allowedFileType: ['image'],
        removeAfterUpload: true,
        autoUpload: false,
        maxFileSize: 10*1024*1024
      });
    }
    
    this.uploader.onAfterAddingAll = (file) => {
      file.withCredentials = false;
    }

    this.uploader.onSuccessItem = (item, responce, status, headers)=>{
      if(responce){
        const photo = JSON.parse(responce);
        this.member.photos.push(photo);
      }
    }
  }
}
