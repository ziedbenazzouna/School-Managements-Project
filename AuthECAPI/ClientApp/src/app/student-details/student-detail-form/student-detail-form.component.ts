import { Component, OnInit, ViewChild } from '@angular/core';
import { StudentDetailService } from '../../shared/services/student-detail.service';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { StudentDetail } from '../../shared/student-detail.model';

@Component({
  selector: 'app-student-detail-form',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './student-detail-form.component.html',
  styles: ``
})
export class StudentDetailFormComponent implements OnInit {
  @ViewChild('form') form!: NgForm
  constructor( public service : StudentDetailService,private toastr: ToastrService){}

  ngOnInit(): void {
    this.service.resetFormSubject.subscribe(() => {
      this.service.resetForm(this.form)
    })
  }

   onSubmit(form: NgForm) {
    this.service.formSubmitted = true
    if (form.valid) {
      if (this.service.formData.studentDetailId == 0)
        this.insertRecord(form)
      else
        this.updateRecord(form)
    }

  }

  insertRecord(form: NgForm) {
    this.service.postStudentDetail()
      .subscribe({
        next: res => {
          this.service.list = res as StudentDetail[]
          this.service.resetForm(form)
          this.toastr.success('Inserted successfully', 'Student Detail Register')
        },
        error: err => {
         
            console.log('error during insert:\n', err); 

        }
      })
  }

  updateRecord(form: NgForm) {
    this.service.putStudentDetail()
      .subscribe({
        next: res => {
          this.service.list = res as StudentDetail[]
          this.service.resetForm(form)
          this.toastr.info('Updated successfully', 'Student Detail Register')
        },
        error: err => { console.log(err) }
      })
   }
}
