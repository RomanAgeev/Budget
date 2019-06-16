[@react.component]
let make = () =>
    <div className="row">
        <form className="col s12">
            <div className="row">
                <div className="input-field col s12">
                    <input id="username" type_="text"/>
                    <label htmlFor="username">{ ReasonReact.string("Username") }</label>
                </div>
            </div>
            <div className="row">
                <div className="input-field col s12">
                    <input id="password" type_="password"/>
                    <label htmlFor="password">{ ReasonReact.string("Password") }</label>
                </div>
            </div>
            <div className="row">
                <div className="input-field col s12">
                    <input id="password_repeat" type_="password"/>
                    <label htmlFor="password_repeat">{ ReasonReact.string("Repeat Password") }</label>
                </div>
            </div>
        </form>
    </div>;