import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  users: any;
  constructor(private http: HttpClient){

  }
  ngOnInit(): void {
    this.getUsers()
  }
  title = 'client';

  getUsers(){
    this.http.get('https://localhost:5001/api/users').subscribe(response=>{
      this.users = response
      console.log(this.users)

    }, error=>console.log('error: ',error)
    )
  }
}
