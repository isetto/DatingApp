import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../models/User';
import { AccountService } from '../services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private accountService: AccountService, private toast: ToastrService){}
  canActivate(): Observable<boolean>{
    return this.accountService.currentUser$.pipe(
      map((user: User)=>{
        if(user) return true
        else this.toast.error("You shall not pass!")
      })
    )
  }
  
}
