import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/models/Member';
import { User } from 'src/app/models/User';
import { AccountService } from 'src/app/services/account.service';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.scss']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm
  member: Member
  user: User
  @HostListener('window:beforeunload', ['$event']) unloadNotofication($event: any){
    if(this.editForm.dirty){
      $event.returnValue = true
    }
  }
  constructor(private accountService: AccountService, private memberService: MembersService, private toast: ToastrService ) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe((user: User)=> {
      this.user = user
    })
  }

  ngOnInit(): void {
    this.loadMember()
  }

  loadMember(){
    this.memberService.getMember(this.user.userName).subscribe((member: Member)=>{
      this.member = member
    })
  }

  updateMember(){
    console.log(this.member)
    this.memberService.updateMember(this.member).subscribe(()=>{
      this.toast.success('Profile updated successfully')
      this.editForm.reset(this.member)
    })
  
  }

}
