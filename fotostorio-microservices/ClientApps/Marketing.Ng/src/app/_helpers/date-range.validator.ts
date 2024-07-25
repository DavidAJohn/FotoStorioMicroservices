import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn } from "@angular/forms";

export function createDateRangeValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const form = control as FormGroup;

        const formStartDate: Date | null = form.get("campaignStartDate")?.value;
        const formEndDate: Date | null = form.get("campaignEndDate")?.value;

        if (formStartDate && formEndDate) {
            const startDate = new Date(formStartDate);
            const endDate = new Date(formEndDate);
            const isRangeValid = (endDate.getTime() - startDate.getTime() > 0);

            form.get("campaignEndDate")?.setErrors(isRangeValid ? null : { dateRange: true });
        }

        return null;
    };
}