import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/models/Member';
import { MembersService } from 'src/app/services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.scss']
})
export class MemberCardComponent implements OnInit {
  @Input() member: Member
  constructor(private memberService: MembersService, private toast: ToastrService) { }

  ngOnInit(): void {
  }

  addLike(member: Member){
    this.memberService.addLike(member.username).subscribe(()=>{
      this.toast.success("You have liked " + member.knownAs)
    })
  }

}
