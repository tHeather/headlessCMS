import { AbstractControl, FormArray, ValidationErrors } from '@angular/forms';

const findDuplicates = (array: string[]) =>
  array.filter(
    (item: string, index: number) => array.lastIndexOf(item) !== index
  );

export function uniqFieldNameValidator(
  control: AbstractControl
): ValidationErrors | null {
  const formArray = control as FormArray;
  const values = formArray.value.map(({ name }: { name: string }) => name);
  const duplicates = findDuplicates(values);

  formArray.controls.forEach((c) => {
    const name = c.get('name');
    const isInvalid = duplicates.includes(name?.value);
    if (isInvalid) name?.setErrors({ uniq: true });
  });

  return duplicates.length < 1 ? null : { uniq: true };
}
