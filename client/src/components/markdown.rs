use regex::Regex;
use yew::{function_component, html, Html, Properties};

#[derive(Properties, PartialEq, Clone)]
pub struct Props {
    pub content: String,
}

#[function_component(Markdown)]
pub fn markdown(props: &Props) -> Html {
    let link_regex = Regex::new(r"\[([^\]]+)\]\((https?://[^\)]+)\)").unwrap();

    let html = props.content.lines().map(|line| {
        let mut line_html = vec![];
        let mut last_pos = 0;
        for chars in link_regex.captures_iter(line) {
            let mat = chars.get(0).unwrap();
            let before_match = &line[last_pos..mat.start()];
            let link_text = &chars[1];
            let link_url = &chars[2];

            if !before_match.is_empty() {
                line_html.push(html! {<>{before_match}</>});
            }

            line_html.push(html! {<a href={link_url.to_string()}>{link_text}</a>});
            last_pos = mat.end();
        }

        if last_pos < line.len() {
            line_html.push(html! {<>{&line[last_pos..]}</>});
        }

        let combined_html = html! { <>{for line_html}</> };

        match line {
            l if l.starts_with("# ") => html! {<h1>{&l[2..]}</h1>},
            l if l.starts_with("## ") => html! {<h2>{&l[3..]}</h2>},
            l if l.starts_with("### ") => html! {<h3>{&l[4..]}</h3>},
            l if l.starts_with("#### ") => html! {<h4>{&l[5..]}</h4>},
            l if l.starts_with("```") => {
                html! {<p><code lang="rust">{line.replace("```", "")}</code></p>}
            }
            l if l.starts_with("---") => html! {<hr/>},
            l if l.starts_with("___") => html! {<hr/>},
            l if l.starts_with("-") => html! {<li>{combined_html}</li>},
            l if l.is_empty() => html! {<br/>},
            _ => html! {<p>{combined_html}</p>},
        }
    });

    html! {
        <div>
            {for html}
        </div>
    }
}
