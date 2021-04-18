import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/models/Member';
import { Pagination } from 'src/app/models/pagination';
import { User } from 'src/app/models/User';
import { UserParams } from 'src/app/models/userParams';
import { AccountService } from 'src/app/services/account.service';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.scss']
})
export class MemberListComponent implements OnInit {
  members: Member[]
  pagination: Pagination
  user: User
  userParams: UserParams
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}]
  constructor(private memberService: MembersService, private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user: User)=> {
      this.user = user
      this.userParams = new UserParams(user)
    })
   }

  ngOnInit(): void {
    this.loadMembers()
  }

  loadMembers(){
    this.memberService.getMembers(this.userParams).subscribe(response =>{
      this.members = response.result
      this.pagination = response.pagination
    })
  }

  pageChanged(event: any){
    this.userParams.pageNumber = event.page
    this.loadMembers()
  }

  resetFilters(){
    this.userParams = new UserParams(this.user);
    this.loadMembers()
  }

}
