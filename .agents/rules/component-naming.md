# Component Naming & Form Behavior Rule

1. **Selector Prefix**: All Angular components in this repository MUST use the `gp-am-` selector prefix (e.g. `gp-am-form`, `gp-am-form-field`, `gp-am-text-input`).
2. **Form Components Naming**: Do NOT use the word "dynamic" when naming form components or services. Use standard terms such as `FormComponent`, `FormFieldComponent`, `FormService`, `FormSchema`, `FormFieldConfig`.
3. **Editable vs Readonly Form Modes**:
   - **Readonly Form Mode** (`isEditable: false`): All components inside the form are readonly/disabled, and labels appear ON TOP (`labelPosition: 'top'`).
   - **Editable Form Mode** (`isEditable: true`): Form components respect their specific `readonly`/`disabled` field configuration, and labels appear TO THE LEFT (`labelPosition: 'left'`).
4. **Mobile First Responsive Design**:
   - All components are styled mobile-first.
   - On small viewports (<640px), left-aligned labels automatically stack above inputs for optimal touch usability and readable layout.
