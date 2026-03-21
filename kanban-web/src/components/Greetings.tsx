interface GreetingsProps {
	greetingText: string;
}

export default function Greetings({ greetingText }: GreetingsProps) {
	return <div>{greetingText}</div>;
}
