import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../models/User';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
model: any = {}
  constructor(public accountService: AccountService, private router: Router, private toast: ToastrService) { }

  ngOnInit(): void {
  }

  login(){
    this.accountService.login(this.model).subscribe(response=>{
      this.router.navigateByUrl("/members")
      this.toast.success('success')
      console.log(response)
    }
    )
  }

  logout(){
    this.accountService.logout()
    this.router.navigateByUrl("/")
  }

}
