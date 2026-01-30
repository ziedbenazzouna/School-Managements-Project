import { Component, OnInit, ViewChild } from '@angular/core';
import { StudentDetailService } from '../../shared/services/student-detail.service';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { StudentDetail } from '../../shared/student-detail.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-student-detail-form',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './student-detail-form.component.html',
  styles: ``
})
export class StudentDetailFormComponent implements OnInit {
  @ViewChild('form') form!: NgForm
  selectedFile: File | null = null;
  imgPreview: string | ArrayBuffer | null = null; // For local preview
  constructor( public service : StudentDetailService,private toastr: ToastrService){}

  ngOnInit(): void {
    this.service.resetFormSubject.subscribe(() => {
      this.service.resetForm(this.form)
      this.imgPreview = null; // Clear local preview when form resets
      this.selectedFile = null;
      // 2. Clear the actual file input in the DOM
    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
    })
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      // Optional: Create a local preview for the user
      const reader = new FileReader();
      reader.onload = () => (this.imgPreview = reader.result);
      reader.readAsDataURL(file);
    }
  }

  onSubmit(form: NgForm) {
    this.service.formSubmitted = true;
    if (form.valid) {
      if (this.selectedFile) {
        // Step 1: Upload image first
        this.service.uploadImage(this.selectedFile).subscribe({
          next: (res) => {
            // Step 2: Assign the returned path to our form data
            this.service.formData.imgPath = res.dbPath;
            this.saveRecord(form);
          },
          error: (err) => this.toastr.error('Image upload failed')
        });
      } else {
        this.saveRecord(form);
      }
    }
  }

  private saveRecord(form: NgForm) {
    if (this.service.formData.studentDetailId == 0) this.insertRecord(form);
    else this.updateRecord(form);
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
