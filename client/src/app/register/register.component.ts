import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter()
  model: any = {}
  constructor(private accountService: AccountService, private toast: ToastrService) { }

  ngOnInit(): void {
  }

  register(){
    this.accountService.register(this.model).subscribe(response=>console.log(response),
    error=>{
      console.log(error)
      this.toast.error(error.error)
    })
    this.cancel()
  }

  cancel(){
    this.cancelRegister.emit(false)
  }

}
