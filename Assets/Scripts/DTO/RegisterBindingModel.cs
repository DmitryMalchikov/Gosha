public class RegisterBindingModel : RegisterExternalBindingModel
{
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    public RegisterBindingModel(RegisterBindingModel model) : base(model)
    {
        Password = model.Password;
        ConfirmPassword = model.Password;
    }

    public RegisterBindingModel() { }
}
