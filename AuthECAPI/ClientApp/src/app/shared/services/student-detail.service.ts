import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { StudentDetail } from '../student-detail.model';
import { NgForm } from '@angular/forms';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StudentDetailService {

  list: StudentDetail[] = [];
  formData: StudentDetail = new StudentDetail()
  formSubmitted: boolean = false;
   resetFormSubject = new Subject<void>()
   private readonly apiBase = environment.apiBaseUrl;

  constructor( private http: HttpClient) { }

  getImageSourceUrl(imgPath: string | undefined | null): string {
    
    if (imgPath && imgPath.length > 0) {
    const backendUrl = environment.apiBaseUrl.replace('/api', ''); // Remove /api suffix if present
    return `${backendUrl}/${imgPath.replace(/\\/g, '/')}`;
  }
  
  // Otherwise, load the avatar from the FRONTEND public folder

  return 'default-image.jpg';
  }

  refreshList() {
    this.http.get(environment.apiBaseUrl+'/studentDetail')
      .subscribe({
        next: res => {
          this.list = res as StudentDetail[]
        },
        error: err => { console.log(err) }
      })
  }

  uploadImage(file: File) {
    const formData = new FormData();
    formData.append('file', file, file.name); // 'file' must match IFormFile parameter name in .NET
    return this.http.post<{ dbPath: string }>(environment.apiBaseUrl + '/upload', formData);
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

    triggerFormReset() {
    this.resetFormSubject.next()
  }

  downloadPdf() {
  return this.http.get(environment.apiBaseUrl + '/StudentDetail/export', {
    responseType: 'blob'
    });
  }

  resetForm(form: NgForm) {
    form.form.reset()
    this.formData = new StudentDetail()
    this.formSubmitted = false
    this.resetFormSubject.next();
  }
}
