import { Component, EventEmitter, Output } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  Validators,
} from '@angular/forms';
import { fieldType } from 'src/app/shared/models/fieldType';
import { uniqFieldNameValidator } from './uniqueFieldName.directive';

@Component({
  selector: 'app-collection-form',
  templateUrl: './collection-form.component.html',
  styleUrls: ['./collection-form.component.scss'],
})
export class CollectionFormComponent {
  isLoading = false;
  fieldTypes = Object.keys(fieldType);
  form = this.fb.group({
    name: ['', Validators.required],
    fields: this.fb.array(
      [
        this.fb.group({
          name: ['', Validators.required],
          fieldType: [fieldType.String, Validators.required],
          isRequierd: [false, Validators.required],
        }),
      ],
      uniqFieldNameValidator
    ),
  });

  @Output() submitFormEvent = new EventEmitter();

  constructor(private fb: FormBuilder) {}

  get fields(): FormArray {
    return this.form.get('fields') as FormArray;
  }

  get name(): FormControl {
    return this.form.get('name') as FormControl;
  }

  addField(): void {
    this.fields.push(
      this.fb.group({
        name: ['', Validators.required],
        fieldType: [fieldType.String, Validators.required],
        isRequierd: [false, Validators.required],
      })
    );
  }

  removeField(index: number): void {
    this.fields.removeAt(index);
  }

  setFormValues(values: any): void {
    this.fields.clear();
    values.fields.forEach(() => {
      this.addField();
    });
    this.form.setValue(values);
  }

  submitForm(): void {
    this.submitFormEvent.emit();
  }
}
