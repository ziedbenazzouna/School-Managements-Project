import { Component, OnInit } from '@angular/core';
import { StudentDetailFormComponent } from './student-detail-form/student-detail-form.component';
import { StudentDetailService } from '../shared/services/student-detail.service';
import { CommonModule } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { StudentDetail } from '../shared/student-detail.model';

@Component({
  selector: 'app-student-details',
  standalone: true,
  imports: [StudentDetailFormComponent,CommonModule],
  templateUrl: './student-details.component.html',
  styles: ``
})
export class StudentDetailsComponent implements OnInit {
  constructor( public service: StudentDetailService, private toastr: ToastrService){}

  ngOnInit(): void {
    this.service.refreshList();
  }

   populateForm(selectedRecord: StudentDetail) {
    this.service.formData = Object.assign({}, selectedRecord);
  }

  onDelete(id: number) {
    if (confirm('Are you sure to delete this record?'))
      this.service.deleteStudentDetail(id)
        .subscribe({
          next: res => {
            this.service.list = res as StudentDetail[]         
            this.toastr.error('Deleted successfully', 'Payment Detail Register')
          },
          error: err => { console.log(err) }
        })
  }

}
