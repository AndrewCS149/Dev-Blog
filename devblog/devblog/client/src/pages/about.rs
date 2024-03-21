use stylist::Style;
use yew::prelude::*;

const STYLE: &str = include_str!("styles/about.css");

#[function_component(About)]
pub fn about() -> Html {
    let style = Style::new(STYLE).unwrap();

    html! {
        <section class={style}>
            <div class="about">
                <ul>
                    <li><a target="_blank" rel="noreferrer" href="https://github.com/AndrewCS149">{"GitHub"}</a></li>
                    <li><a target="_blank" rel="noreferrer" href="https://mastodon.social/@TheDevBlog">{"Mastodon"}</a></li>
                    <li><a target="_blank" rel="noreferrer" href="https://andrew149.itch.io/">{"Itch.io"}</a></li>
                    // <li><a target="_blank" rel="noreferrer" href="https://discord.gg/uHDwywr6wt">Discord</a></li>
                    <li><a target="_blank" rel="noreferrer" href="https://www.linkedin.com/in/andrew149/">{"LinkedIn"}</a></li>
                </ul>

                <img src="./assets/imgs/me.png" alt="The DevBlog creator Andrew Smith" />

            </div>
        </section>
    }
}
