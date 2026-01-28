import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { StudentDetail } from '../student-detail.model';
import { NgForm } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class StudentDetailService {

  list: StudentDetail[] = [];
  formData: StudentDetail = new StudentDetail()
  formSubmitted: boolean = false;

  constructor( private http: HttpClient) { }

  refreshList() {
    this.http.get(environment.apiBaseUrl+'/studentDetail')
      .subscribe({
        next: res => {
          this.list = res as StudentDetail[]
        },
        error: err => { console.log(err) }
      })
  }

  postStudentDetail() {
    return this.http.post(environment.apiBaseUrl+'/studentDetail', this.formData)
  }

  putStudentDetail() {
    return this.http.put(environment.apiBaseUrl+'/studentDetail' + '/' + this.formData.studentDetailId, this.formData)
  }


  deleteStudentDetail(id: number) {
    return this.http.delete(environment.apiBaseUrl+'/studentDetail' + '/' + id)
  } 


  resetForm(form: NgForm) {
    form.form.reset()
    this.formData = new StudentDetail()
    this.formSubmitted = false
  }
}
