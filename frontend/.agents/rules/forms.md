# Form Validation Rules

1. All forms must enforce complete form validation before enabling form submission.
2. Submit buttons must be disabled (`[disabled]="form.invalid || isLoading()"`) and styled with a muted/greyed-out disabled state until all required form fields are valid.
3. Form submissions should check `if (this.form.invalid) return;` as a guard clause before triggering API or service calls.
