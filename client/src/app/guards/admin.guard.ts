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
export class AdminGuard implements CanActivate {
  constructor(private accountService: AccountService, private toast: ToastrService){}
  canActivate(): Observable<boolean> {
    return this.accountService.currentUser$.pipe(map((user: User)=>{
      if(user.roles.includes('Admin') || user.roles.includes('Moderator')) return true
      else this.toast.error('You cannot enter this area')
    }));
  }
  
}
